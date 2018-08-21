using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepData;

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

        public ChartMetadata ChartMetadata { get; private set; }
        
        private IDictionary<SmFileAttribute, string> _attributes;
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

            ChartMetadata = new ChartMetadata();
            _attributes = new Dictionary<SmFileAttribute, string>();

            ParseFile();
        }

        public void Refresh()
        {
            ChartMetadata = new ChartMetadata();
            _attributes = new Dictionary<SmFileAttribute, string>();

            ParseFile();
        }

        private void ParseFile()
        {
            var buffer = new StringBuilder();
        
            using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    if (reader.Peek() == ':')
                    {
                        //buffer contains tag in the format #TAG
                        var tag = buffer.SkipWhile(c => c != '#').ToString().Trim('#').ToAttribute();

                        if (tag != SmFileAttribute.UNDEFINED)
                        {
                            if (tag == SmFileAttribute.NOTES)
                            {
                                //Parse chart metadata
                                var stepData = ReadStepchartMetadata(reader, buffer);
                                ChartMetadata.Add(stepData);

                                //skip the stream reader ahead to the next tag
                                while (reader.Peek() != ';') reader.Read();
                            }
                            else
                            {
                                var value = ReadTagValue(reader, buffer);
                                _attributes.Add(tag, value);
                            }
                        }
                        else
                        {
                            //could not read tag, toss the ':' token and continue
                            reader.Read();
                        }

                        buffer.Clear();
                    }
                    else
                    {
                        buffer.Append((char)reader.Read());
                    }
                }
            }
        }

        public void AddLightsChart(LightsChart chart)
        {
            using (var stream = new FileStream(FilePath, FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(chart.Content);
            }
        }

        internal static string ReadTagValue(StreamReader reader, StringBuilder buffer)
        {
            reader.Read(); //toss ':' token
            buffer.Clear();
            while (reader.Peek() != ';')
            {
                buffer.Append((char) reader.Read());
            }

            return buffer.ToString();
        }

        internal static StepMetadata ReadStepchartMetadata(StreamReader reader, StringBuilder buffer)
        {
            reader.Read(); //toss ':' token

            var stepData = new StepMetadata
            {
                PlayStyle = ReadNextNoteHeaderSection(reader, buffer).ToStyleEnum(),
                ChartAuthor = ReadNextNoteHeaderSection(reader, buffer),
                Difficulty = ReadNextNoteHeaderSection(reader, buffer).ToSongDifficultyEnum(),
                DifficultyRating = (int) double.Parse(ReadNextNoteHeaderSection(reader, buffer))
            };
            
            return stepData;
        }

        internal static string ReadNextNoteHeaderSection(StreamReader reader, StringBuilder buffer)
        {
            buffer.Clear();
            while (reader.Peek() != ':')
            {
                buffer.Append((char)reader.Read());
            }
            reader.Read(); //toss ':' token

            return buffer.SkipWhile(char.IsWhiteSpace).ToString();
        }
    }
}
