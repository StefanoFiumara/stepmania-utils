using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepChart;

namespace StepmaniaUtils.Core
{
    public class SmFile
    {
        public string SongName { get; set; }
        public string Group { get; set; }
        public string BannerPath { get; set; }
        public string Artist { get; set; }
        public string Directory { get; set; }
        
        public string FilePath { get; }

        public SmFile(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.GetFullPath(filePath);
            }

            if (File.Exists(filePath) == false || !filePath.ToLower().EndsWith(".sm"))
            {
                throw new ArgumentException($"The given .sm file path is either invalid or a file was not found. Path: {filePath}");
            }

            FilePath = filePath;

            Group = Path.GetFullPath(Path.Combine(filePath, @"..\.."))
                        .Split(Path.DirectorySeparatorChar)
                        .Last();

            Directory = Path.GetDirectoryName(filePath);

            SongName = GetAttribute(SmFileAttribute.TITLE);
            Artist = GetAttribute(SmFileAttribute.ARTIST);
            BannerPath = GetAttribute(SmFileAttribute.BANNER);

            if (Path.HasExtension(BannerPath) == false)
            {
                BannerPath += ".png";
            }
        }
        
        public ChartData ExtractChartData(bool extractAllStepData = true)
        {
            var stepCharts = new List<StepData>();
            var fileContent = File.ReadLines(FilePath).ToList();

            for (int i = 0; i < fileContent.Count; i++)
            {
                if (!fileContent[i].Contains("#NOTES:")) continue;

                string styleLine = fileContent[i + 1];
                string author = fileContent[i + 2].Trim().TrimEnd(':');
                string difficultyLine = fileContent[i + 3];
                int rating = (int)double.Parse(fileContent[i + 4].Trim().TrimEnd(':'));

                PlayStyle style = EnumExtensions.ToStyleEnum(styleLine);
                SongDifficulty difficulty = EnumExtensions.ToSongDifficultyEnum(difficultyLine);

                int noteDataStartIndex = i + 6;
                //Stupid Edge case
                if (fileContent[i + 5].Trim().EndsWith(":") == false)
                {
                    var nextLine = string.Concat(fileContent[i + 5].Trim().SkipWhile(c => c != ':').Skip(1));
                    fileContent.Insert(i + 6, nextLine);
                }

                int noteDataEndIndex = noteDataStartIndex;

                while (fileContent[noteDataEndIndex].Contains(";") == false) noteDataEndIndex++;

                if (extractAllStepData)
                {
                    var rawNoteData = fileContent.Skip(noteDataStartIndex).Take(noteDataEndIndex - noteDataStartIndex).ToList();
                    stepCharts.Add(new StepData(style, difficulty, rating, author, rawNoteData));
                }
                else
                {
                    stepCharts.Add(new StepData(style, difficulty, rating, author));
                }

                i = noteDataEndIndex;
            }

            return new ChartData(stepCharts, FilePath);
        }
        
        public string GetAttribute(SmFileAttribute attribute)
        {
            string attributeName = attribute.ToString();

            var fileContent = File.ReadLines(FilePath);

            string attributeLine = fileContent
                                    .TakeWhile(s => s.Contains("#NOTES:") == false)
                                    .FirstOrDefault(line => line.Contains($"#{attributeName}:"));

            if (attributeLine != null)
            {
                return attributeLine
                    .Replace($"#{attributeName}:", string.Empty)
                    .TrimEnd(';');
            }

            return string.Empty;
        }
    }
}
