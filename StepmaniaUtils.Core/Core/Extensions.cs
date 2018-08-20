using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepmaniaUtils.Core
{
    public static class Extensions
    {
        public static string AsString(this IEnumerable<char> sequence)
        {
            var sb = new StringBuilder();

            foreach (var c in sequence)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}