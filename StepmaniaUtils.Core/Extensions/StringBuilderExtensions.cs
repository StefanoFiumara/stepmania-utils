using System;
using System.Text;

namespace StepmaniaUtils.Extensions
{
    internal static class StringBuilderExtensions
    {
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