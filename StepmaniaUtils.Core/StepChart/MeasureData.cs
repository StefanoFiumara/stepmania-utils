using System;
using System.Collections.Generic;
using System.Linq;
using StepmaniaUtils.Core;

namespace StepmaniaUtils.StepChart
{
    public class MeasureData : IDisposable
    {
        public List<ColumnData> Notes { get; set; }

        public MeasureData()
        {
            this.Notes = new List<ColumnData>();
        }
        public MeasureData(string measureData)
        {
            this.Notes = this.ParseRawMeasureData(measureData);
        }

        private List<ColumnData> ParseRawMeasureData(string measureData)
        {
            return measureData.Split('\n')
                .Select(data => data.Trim())
                .Where(data => !data.Contains(@"//"))
                .Where(data => string.IsNullOrWhiteSpace(data) == false)
                .Select(data => new ColumnData(data))
                .ToList();
        }

        public void Dispose()
        {
            this.Notes = null;
        }
    }
}