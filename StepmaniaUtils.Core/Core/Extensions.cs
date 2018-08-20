using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StepmaniaUtils.Core
{
    internal static class Extensions
    {
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
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