using System.Diagnostics;

namespace StepmaniaUtils.StepChart
{
    [DebuggerDisplay("{" + nameof(Columns) + "}")]
    public class ColumnData
    {
        public string Columns { get; }

        public ColumnData(string columns)
        {
            this.Columns = columns;
        }
    }
}