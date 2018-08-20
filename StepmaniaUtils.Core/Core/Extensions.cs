using System;
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

        public static StringBuilder SkipWhile(this StringBuilder sb, Func<char, bool> condition)
        {
            int skipCount = 0;

            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];

                if (condition(c))
                {
                    skipCount++;
                }
                else
                {
                    break;
                }
            }

            if (skipCount > 0)
            {
                sb.Remove(0, skipCount);
            }

            return sb;
        }
    }
}