using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepChart;

namespace StepmaniaUtils.Core
{
    public class SmFile
    {
        
        public string SongName => Attributes.ContainsKey(SmFileAttribute.TITLE) ? Attributes[SmFileAttribute.TITLE] : string.Empty;
        public string BannerPath => Attributes.ContainsKey(SmFileAttribute.BANNER) ? Attributes[SmFileAttribute.BANNER] : string.Empty;
        public string Artist => Attributes.ContainsKey(SmFileAttribute.ARTIST) ? Attributes[SmFileAttribute.ARTIST] : string.Empty;

        public string Directory { get; }
        public string Group { get; }
        public string FilePath { get; }

        private IReadOnlyDictionary<SmFileAttribute, string> Attributes { get; }

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

            Attributes = ReadAttributes();
        }

        private IReadOnlyDictionary<SmFileAttribute, string> ReadAttributes()
        {
            var attributes = new Dictionary<SmFileAttribute, string>();

            var tagBuffer = new StringBuilder();
            var valueBuffer = new StringBuilder();

            using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    if (reader.Peek() == ':')
                    {
                        //buffer contains tag in the format #TAG
                        var tag = tagBuffer.ToString().SkipWhile(c => c != '#').AsString().Trim('#').ToAttribute();
                        
                        if (tag != SmFileAttribute.UNDEFINED && tag != SmFileAttribute.NOTES)
                        {
                            //Read the tag's value
                            reader.Read(); //toss ':' token
                            valueBuffer.Clear();
                            while (reader.Peek() != ';')
                            {
                                valueBuffer.Append((char)reader.Read());
                            }

                            var value = valueBuffer.ToString();

                            attributes.Add(tag, value);
                        }

                        tagBuffer.Clear();

                        //TODO: parse notes section for chart metadata
                        if (tag == SmFileAttribute.NOTES) break;
                    }
                    else
                    {
                        tagBuffer.Append((char)reader.Read());
                    }
                }
            }

            return attributes;
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
            if (Attributes.ContainsKey(attribute))
            {
                return Attributes[attribute];
            }

            return string.Empty;
        }
    }
}
