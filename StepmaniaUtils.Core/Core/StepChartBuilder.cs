using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepData;

namespace StepmaniaUtils.Core
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
            
            return new LightsChart(lightsData);
        }

        private static string GenerateLightsChart(string file, StepMetadata referenceData)
        {
            var buffer = new StringBuilder();
            
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    if (reader.Peek() == ':')
                    {
                        //buffer contains tag in the format #TAG
                        var tag = buffer.SkipWhile(c => c != '#').ToString().Trim('#').ToAttribute();
                        
                        if (tag == SmFileAttribute.NOTES)
                        {
                            var stepData = SmFile.ReadStepchartMetadata(reader, buffer);
                            if (stepData.PlayStyle == referenceData.PlayStyle && stepData.Difficulty == referenceData.Difficulty)
                            {
                                //Skip groove radar values
                                SmFile.ReadNextNoteHeaderSection(reader, buffer);

                                buffer.Clear();

                                return GenerateLightsChart(reader, buffer);
                            }
                            //wrong chart, skip the stream reader ahead to the next tag
                            while (reader.Peek() != ';') reader.Read();
                        }
                        else
                        {   //skip
                            while (reader.Peek() != ';') reader.Read();
                        }

                        buffer.Clear();

                    }
                    else
                    {
                        buffer.Append((char)reader.Read());
                    }
                }
            }

            throw new Exception($"Could not find note data to reference in {file}");
        }

        private static string GenerateLightsChart(StreamReader reader, StringBuilder buffer)
        {
            var result = new StringBuilder()
                  .AppendLine($"//---------------lights-cabinet-----------------")
                  .AppendLine($"#NOTES:")
                  .AppendLine($"    lights-cabinet:")
                  .AppendLine($"    auto-generated:")
                  .AppendLine($"    Easy:")
                  .AppendLine($"    1:")
                  .AppendLine($"    0.000,0.000,0.000,0.000,0.000:");
            
            var measureData = new List<string>(192);

            bool isHolding = false;

            while (reader.Peek() != ';')
            {
                buffer.Clear();
                measureData.Clear();

                while (reader.Peek() != ',' && reader.Peek() != ';') buffer.Append((char) reader.Read());

                var measureLines = buffer.ToString().Split(Environment.NewLine.ToCharArray())
                                                    .Select(data => data.Trim())
                                                    .Where(data => !data.Contains(@"//"))
                                                    .Where(data => !string.IsNullOrWhiteSpace(data));

                measureData.AddRange(measureLines);

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

                if (reader.Peek() == ';')
                {
                    result.AppendLine(";");
                }
                else
                {
                    result.AppendLine(",");
                    reader.Read(); //consume delimiter.
                }
            }

            return result.ToString();
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