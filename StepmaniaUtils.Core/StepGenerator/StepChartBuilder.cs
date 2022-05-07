using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.Readers;
using StepmaniaUtils.StepData;

namespace StepmaniaUtils.StepGenerator
{
    public static class StepChartBuilder
    {
        public static LightsChart GenerateLightsChart(SmFile file)
        {
            var reference = file.ChartMetadata.GetSteps(PlayStyle.Single, SongDifficulty.Hard)
                         ?? file.ChartMetadata.GetSteps(PlayStyle.Single, SongDifficulty.Challenge)
                         ?? file.ChartMetadata.GetSteps(PlayStyle.Double, SongDifficulty.Hard)
                         ?? file.ChartMetadata.GetSteps(PlayStyle.Double, SongDifficulty.Challenge)
                         ?? file.ChartMetadata.GetSteps(PlayStyle.Single, file.ChartMetadata.GetHighestChartedDifficulty(PlayStyle.Single))
                         ?? file.ChartMetadata.GetSteps(PlayStyle.Double, file.ChartMetadata.GetHighestChartedDifficulty(PlayStyle.Double));

            if (reference == null)
                throw new ArgumentException("Could not find a reference chart.", nameof(file));

            var lightsData = GenerateLightsChart(file.FilePath, reference);

            return new LightsChart(lightsData, reference);
        }

        private static string GenerateLightsChart(string file, StepMetadata referenceData)
        {
            using (var reader = StepmaniaFileReaderFactory.CreateReader(file))
            {
                while (reader.ReadNextTag(out SmFileAttribute tag))
                {
                    if(reader.State != ReaderState.ReadingChartMetadata)
                        continue;

                    var stepData = reader.ReadStepchartMetadata();

                    if (stepData.PlayStyle == referenceData.PlayStyle && stepData.Difficulty == referenceData.Difficulty)
                    {
                        return GenerateLightsChart(reader);
                    }
                }
            }

            throw new Exception($"Could not find note data to reference in {file}");
        }

        private static string GenerateLightsChart(IStepmaniaFileReader reader)
        {
            var result = GetLightsChartHeader(reader.FilePath);

            var measureData = new List<string>(192);

            bool isHolding = false;

            while (reader.IsParsingNoteData)
            {
                measureData.Clear();
                measureData.AddRange( reader.ReadMeasure() );

                int quarterNoteBeatIndicator = measureData.Count / 4;
                int noteIndex = 0;

                foreach (string note in measureData)
                {
                    string marqueeLights = note.Replace('M', '0'); //ignore mines
                    if (note.Length > 4)
                    {
                        marqueeLights = MapDoubles(marqueeLights);
                    }

                    bool isQuarterBeat = noteIndex % quarterNoteBeatIndicator == 0;
                    bool hasNote = marqueeLights.Any(c => c != '0');
                    bool isHoldBegin = marqueeLights.Any(c => c == '2' || c == '4');
                    bool isHoldEnd = marqueeLights.Any(c => c == '3');
                    bool isJump = marqueeLights.Count(c => c != '0') >= 2;

                    string bassLights = (hasNote && isQuarterBeat) || isJump ? "11" : "00";

                    if (isHoldBegin && !isHolding)
                    {
                        bassLights = "22"; //hold start
                        isHolding = true;
                    }
                    else if (isHolding)
                    {
                        bassLights = "00"; //ignore beats if there is a hold
                    }

                    if (isHoldEnd && !isHoldBegin)
                    {
                        bassLights = "33"; //hold end
                        isHolding = false;
                    }

                    result.AppendLine($"{marqueeLights}{bassLights}00");
                    noteIndex++;
                }

                //Append a ',' if there are more measures to parse
                //Otherwise append a ';' to dictate the end of the chart
                result.AppendLine(reader.IsParsingNoteData ? "," : ";");
            }

            return result.ToString();
        }

        private static StringBuilder GetLightsChartHeader(string smFilePath)
        {
            if (Path.GetExtension(smFilePath) == ".sm")
            {
                return new StringBuilder()
                    .AppendLine("//---------------lights-cabinet-----------------")
                    .AppendLine("#NOTES:")
                    .AppendLine("    lights-cabinet:")
                    .AppendLine("    auto-generated:")
                    .AppendLine("    Easy:")
                    .AppendLine("    1:")
                    .AppendLine("    0.000,0.000,0.000,0.000,0.000:");
            }
            else
            {
                return new StringBuilder()
                    .AppendLine("//---------------lights-cabinet-----------------")
                    .AppendLine("#NOTEDATA:;")
                    .AppendLine("#CHARTNAME:lights;")
                    .AppendLine("#STEPSTYPE:lights-cabinet;")
                    .AppendLine("#DESCRIPTION:auto-generated;")
                    .AppendLine("#CHARTSTYLE:;")
                    .AppendLine("#DIFFICULTY:Easy;")
                    .AppendLine("#METER:1;")
                    .AppendLine("#RADARVALUES:0.000,0.000,0.000,0.000,0.000;")
                    .AppendLine("#CREDIT:Fano;") // ;)
                    .AppendLine("#DISPLAYBPM:120.00;")
                    .AppendLine("#NOTES:");
            }
        }

        private static string MapDoubles(string marqueeLights)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                char note = '0';

                int p1 = i;
                int p2 = i + 4;

                if (marqueeLights[p1] != '0') note = marqueeLights[p1];
                if (marqueeLights[p2] != '0') note = marqueeLights[p2];

                sb.Append(note);
            }

            return sb.ToString();
        }
    }
}
