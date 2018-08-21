using System;
using System.Collections.Generic;
using System.Linq;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.StepData
{
    public class ChartMetadata
    {
        private readonly List<StepMetadata> _stepCharts;

        public IReadOnlyList<StepMetadata> StepCharts => _stepCharts.AsReadOnly();

        public ChartMetadata()
        {
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