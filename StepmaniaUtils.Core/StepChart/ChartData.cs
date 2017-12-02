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
        private FileInfo SmFile { get; }

        private List<StepData> NewStepData { get; set; }
        private bool IsDirty { get; set; }

        public ChartData(List<StepData> stepCharts, FileInfo smFile)
        {
            this.StepCharts = stepCharts;
            this.SmFile = smFile;
            this.NewStepData = new List<StepData>();
        }

        public StepData GetSteps(PlayStyle style, SongDifficulty difficulty)
        {
            return this.StepCharts.FirstOrDefault(c => c.PlayStyle == style && c.Difficulty == difficulty);
        }

        public SongDifficulty GetHighestChartedDifficulty(PlayStyle style)
        {
            return
                this.StepCharts
                    .Where(c => c.PlayStyle == style)
                    .OrderByDescending(c => c.Difficulty)
                    .Select(d => d.Difficulty)
                    .FirstOrDefault();
        }

        public void AddNewStepchart(StepData chartData)
        {
            if (chartData == null)
                throw new ArgumentNullException(nameof(chartData), "Attempted to add null chart data.");

            if (this.GetSteps(chartData.PlayStyle, chartData.Difficulty) != null)
            {
                string exMessage = $"This CharData already contains a stepchart for {chartData.PlayStyle}-{chartData.Difficulty}";
                throw new InvalidOperationException(exMessage);
            }

            this.StepCharts.Add(chartData);
            this.NewStepData.Add(chartData);
            this.IsDirty = true;

           
        }

        public void Dispose()
        {
            if (this.IsDirty)
            {
                var backupFilePath = this.SmFile.FullName + ".backup";
                File.Copy(this.SmFile.FullName, backupFilePath, true);

                var rawStepData = this.NewStepData.SelectMany(d => d.GetRawChartData());
                File.AppendAllLines(this.SmFile.FullName, rawStepData);
            }

            foreach (var stepChart in this.StepCharts)
            {
                stepChart.Dispose();
            }

            this.StepCharts = null;
        }
    }
}