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
        public string Directory => this.SmFileInfo.DirectoryName;

        private FileInfo SmFileInfo { get; set; }

        public SmFile(FileInfo smFile)
        {
            this.SmFileInfo = smFile;

            if (this.SmFileInfo.Exists == false || this.SmFileInfo.Extension.ToLower() != ".sm")
            {
                throw new ArgumentException($"The given .sm file path is either invalid or a file was not found. Path: {this.SmFileInfo.FullName}");
            }

            this.SongName = this.GetAttribute(SmFileAttribute.TITLE);
            this.Group = this.SmFileInfo.Directory?.Parent?.Name ?? string.Empty;
            this.BannerPath = this.GetAttribute(SmFileAttribute.BANNER);
            if (Path.HasExtension(this.BannerPath) == false) this.BannerPath += ".png";
            this.Artist = this.GetAttribute(SmFileAttribute.ARTIST);
        }
        
        public ChartData ExtractChartData()
        {
            var stepCharts = new List<StepData>();
            var fileContent = File.ReadLines(this.SmFileInfo.FullName).ToList();

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

                var rawNoteData =
                    fileContent.Skip(noteDataStartIndex)
                        .Take(noteDataEndIndex - noteDataStartIndex)
                        .ToList();

                stepCharts.Add(new StepData(style, difficulty, rating, author, rawNoteData));

                i = noteDataEndIndex;
            }

            return new ChartData(stepCharts, this.SmFileInfo);
        }

        public string GetAttribute(SmFileAttribute attribute)
        {
            string attributeName = attribute.ToString();

            var fileContent = File.ReadLines(this.SmFileInfo.FullName);

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
