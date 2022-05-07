using StepmaniaUtils.StepData;

namespace StepmaniaUtils.StepGenerator
{
    public class LightsChart
    {
        public string Content { get; }
        public StepMetadata ReferenceChart { get; }


        internal LightsChart(string content, StepMetadata referenceChart)
        {
            Content = content;
            ReferenceChart = referenceChart;
        }
    }
}
