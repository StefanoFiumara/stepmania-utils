using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepChart;

namespace StepmaniaUtils.Core
{
    public class SmFile
    {
        public string this[SmFileAttribute attribute] =>
            Attributes.ContainsKey(attribute) ? Attributes[attribute] : string.Empty;

        public string SongTitle => this[SmFileAttribute.TITLE];
        public string BannerPath => this[SmFileAttribute.BANNER];
        public string Artist => this[SmFileAttribute.ARTIST];

        public string Directory { get; }
        public string Group { get; }
        public string FilePath { get; }

        public ChartMetadata ChartMetadata { get; }
        
        private readonly IDictionary<SmFileAttribute, string> _attributes;
        public IReadOnlyDictionary<SmFileAttribute, string> Attributes => _attributes.AsReadOnly();
        

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

            ChartMetadata = new ChartMetadata(FilePath);
            _attributes = new Dictionary<SmFileAttribute, string>();

            ParseFile();
        }

        private void ParseFile()
        {
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
                        var tag = tagBuffer.SkipWhile(c => c != '#').ToString().Trim('#').ToAttribute();
                        
                        if (tag != SmFileAttribute.UNDEFINED)
                        {
                            if (tag == SmFileAttribute.NOTES)
                            {
                                //Parse chart metadata
                                var stepData = ReadStepchartMetadata(reader, valueBuffer);
                                ChartMetadata.Add(stepData);
                            }
                            else
                            {
                                var value = ReadTagValue(reader, valueBuffer);
                                _attributes.Add(tag, value);
                            }
                        }

                        tagBuffer.Clear();
                    }
                    else
                    {
                        tagBuffer.Append((char)reader.Read());
                    }
                }
            }
        }

        private static string ReadTagValue(StreamReader reader, StringBuilder buffer)
        {
            reader.Read(); //toss ':' token
            buffer.Clear();
            while (reader.Peek() != ';')
            {
                buffer.Append((char) reader.Read());
            }

            return buffer.ToString();
        }

        private static StepMetadata ReadStepchartMetadata(StreamReader reader, StringBuilder buffer)
        {
            reader.Read(); //toss ':' token

            var stepData = new StepMetadata
            {
                PlayStyle = ReadNextNoteHeaderSection(reader, buffer).ToStyleEnum(),
                ChartAuthor = ReadNextNoteHeaderSection(reader, buffer),
                Difficulty = ReadNextNoteHeaderSection(reader, buffer).ToSongDifficultyEnum(),
                DifficultyRating = (int) double.Parse(ReadNextNoteHeaderSection(reader, buffer))
            };

            //skip the stream reader ahead to the next tag
            while (reader.Peek() != ';') reader.Read();

            return stepData;
        }

        private static string ReadNextNoteHeaderSection(StreamReader reader, StringBuilder buffer)
        {
            buffer.Clear();
            while (reader.Peek() != ':')
            {
                buffer.Append((char)reader.Read());
            }
            reader.Read(); //toss ':' token

            return buffer.SkipWhile(char.IsWhiteSpace).ToString();
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

                PlayStyle style = styleLine.ToStyleEnum();
                SongDifficulty difficulty = difficultyLine.ToSongDifficultyEnum();

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
    }
}
