using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepData;
using StepmaniaUtils.StepGenerator;

namespace StepmaniaUtils.Core
{
    public class SmFile
    {
        public string this[SmFileAttribute attribute] =>
            Attributes.ContainsKey(attribute) ? Attributes[attribute] : string.Empty;

        public string SongTitle => this[SmFileAttribute.TITLE];
        public string Artist => this[SmFileAttribute.ARTIST];
        
        public string Directory { get; }
        public string BannerPath => this[SmFileAttribute.BANNER];
        public string Group { get; }
        public string FilePath { get; }
        public string DisplayBpm { get; private set; }

        public ChartMetadata ChartMetadata { get; private set; }
        
        private IDictionary<SmFileAttribute, string> _attributes;

        public IReadOnlyDictionary<SmFileAttribute, string> Attributes => new ReadOnlyDictionary<SmFileAttribute, string>(_attributes);
        
        public SmFile(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.GetFullPath(filePath);
            }

            var validExtensions = new[] {".sm", ".ssc"};

            if (File.Exists(filePath) == false || !validExtensions.Contains(Path.GetExtension(filePath)))
            {
                throw new ArgumentException($"The given .sm or .ssc file path is either invalid or a file was not found. Path: {filePath}");
            }

            FilePath = filePath;

            Group = Path.GetFullPath(Path.Combine(filePath, @"..\.."))
                        .Split(Path.DirectorySeparatorChar)
                        .Last();

            Directory = Path.GetDirectoryName(filePath);

            ChartMetadata = new ChartMetadata();
            _attributes = new Dictionary<SmFileAttribute, string>();

            ParseFile();
            SetDisplayBpm();
        }

        private void SetDisplayBpm()
        {
            if (_attributes.TryGetValue(SmFileAttribute.DISPLAYBPM, out string displayBpm) && displayBpm != "*" && displayBpm != "?")
            {
                if (displayBpm.Contains(':'))
                {
                    var bpms = displayBpm.Split(':').Select(double.Parse).ToList();

                    var highest = bpms.Max();
                    var lowest = bpms.Min();

                    DisplayBpm = $"{lowest:####}-{highest:####}";
                }
                else
                {
                    DisplayBpm = $"{double.Parse(displayBpm):####}";
                }
            }
            else
            {
                var bpms = _attributes[SmFileAttribute.BPMS].Split(',').Select(t => t.Split('=')[1]).Select(double.Parse).ToList();

                var highest = bpms.Max();
                var lowest = bpms.Min();

                DisplayBpm = Math.Abs(highest - lowest) > 0.01f ? $"{lowest:####}-{highest:####}" : $"{highest:####}";
            }
        }

        public void Refresh()
        {
            ChartMetadata = new ChartMetadata();
            _attributes = new Dictionary<SmFileAttribute, string>();

            ParseFile();
            SetDisplayBpm();
        }

        private void ParseFile()
        {
            using (var reader = new SmFileReader(FilePath))
            {
                while (reader.ReadNextTag(out SmFileAttribute tag))
                {
                    if (tag == SmFileAttribute.NOTES)
                    {
                        var stepData = reader.ReadStepchartMetadata();

                        ChartMetadata.Add(stepData);

                        reader.SkipValue();
                    }
                    else
                    {
                        var value = reader.ReadTagValue();

                        if (!_attributes.ContainsKey(tag))
                        {
                            _attributes.Add(tag, value);
                        }
                    }
                }
            }
        }

        public void AddLightsChart(LightsChart chart)
        {
            //TODO: Backup file
            using (var stream = new FileStream(FilePath, FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(chart.Content);
            }
        }
    }
}
