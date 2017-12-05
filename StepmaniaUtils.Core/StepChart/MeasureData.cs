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
            Notes = new List<ColumnData>();
        }
        public MeasureData(string measureData)
        {
            Notes = ParseRawMeasureData(measureData);
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
            Notes = null;
        }
    }
}