using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.StepChart
{
    public class ChartData : IDisposable
    {
        private List<StepData> StepCharts { get; set; }
        public string SmFilePath { get; }
        
        private List<StepData> NewStepData { get; }
        private bool IsDirty { get; set; }

        public ChartData(List<StepData> stepCharts, string smFilePath)
        {
            StepCharts = stepCharts;
            SmFilePath = smFilePath;

            NewStepData = new List<StepData>();
        }

        public StepData GetSteps(PlayStyle style, SongDifficulty difficulty)
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

        public void AddNewStepchart(StepData chartData)
        {
            if (chartData == null)
                throw new ArgumentNullException(nameof(chartData), "Attempted to add null chart data.");

            if (GetSteps(chartData.PlayStyle, chartData.Difficulty) != null)
            {
                string exMessage = $"This CharData already contains a stepchart for {chartData.PlayStyle}-{chartData.Difficulty}";
                throw new InvalidOperationException(exMessage);
            }

            StepCharts.Add(chartData);
            NewStepData.Add(chartData);
            IsDirty = true;

           
        }

        public void Dispose()
        {
            if (IsDirty)
            {
                var backupFilePath = SmFilePath + ".backup";
                File.Copy(SmFilePath, backupFilePath, true);

                var rawStepData = NewStepData.SelectMany(d => d.GetRawChartData());
                File.AppendAllLines(SmFilePath, rawStepData);
            }

            foreach (var stepChart in StepCharts)
            {
                stepChart.Dispose();
            }

            StepCharts = null;
        }
    }
}