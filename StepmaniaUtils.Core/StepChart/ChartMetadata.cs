using System.Collections.Generic;
using System.Linq;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.StepChart
{
    public class ChartMetadata
    {
        private readonly string _filePath;
        private readonly List<StepMetadata> _stepCharts;

        private IReadOnlyList<StepMetadata> StepCharts => _stepCharts.AsReadOnly();

        public ChartMetadata(string filePath)
        {
            _filePath = filePath;
            _stepCharts = new List<StepMetadata>();
        }

        internal void Add(StepMetadata stepData)
        {
            _stepCharts.Add(stepData);
        }

        public StepMetadata GetSteps(PlayStyle style, SongDifficulty difficulty)
        {
            return StepCharts.FirstOrDefault(c => c.PlayStyle == style && c.Difficulty == difficulty);
        }

        public SongDifficulty GetHighestChartedDifficulty(PlayStyle style)
        {
            return
                StepCharts
                    .Where(c => c.PlayStyle == style)
                    .OrderByDescending(c => c.Difficulty)
                    .Select(d => d.Difficulty)
                    .FirstOrDefault();
        }
    }
}