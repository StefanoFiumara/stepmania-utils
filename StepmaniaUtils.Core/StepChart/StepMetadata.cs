using System.Diagnostics;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.StepChart
{
    [DebuggerDisplay("{PlayStyle} - {Difficulty} - {DifficultyRating} - {ChartAuthor}")]
    public class StepMetadata
    {
        //TODO: Store some way to figure out where the note data starts in the .sm file
        public PlayStyle PlayStyle { get; set; }
        public SongDifficulty Difficulty { get; set; }
        public int DifficultyRating { get; set; }
        public string ChartAuthor { get; set; }
    }
}