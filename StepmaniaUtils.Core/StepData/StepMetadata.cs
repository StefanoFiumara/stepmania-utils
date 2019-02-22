using System.Diagnostics;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.StepData
{
    [DebuggerDisplay("{PlayStyle} - {Difficulty} - {DifficultyRating} - {ChartAuthor}")]
    public class StepMetadata
    {
        internal StepMetadata() { }

        public PlayStyle PlayStyle { get; set; }
        public SongDifficulty Difficulty { get; set; }
        public int DifficultyRating { get; set; }
        public string ChartAuthor { get; set; }

        public string ChartName { get; set; }
    }
}