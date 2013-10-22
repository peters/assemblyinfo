#if NET20
using System;
using System.Collections.Generic;

namespace assemblyinfo.Extensions
{
    internal static class LinqExtensions
    {
        public static bool Any<T>(this IEnumerable<T> values, Mono.Func<T, bool> predicate)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in values)
            {
                if (predicate(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static int Max<T>(this IEnumerable<T> values, Mono.Func<T, int> predicate) where T: struct
        {
            var maxValue = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in values)
            {
                var convertedValue = (int)Convert.ChangeType(value, typeof(int));
                if (convertedValue > maxValue)
                {
                    maxValue = convertedValue;
                }
            }
            return maxValue;
        }

        public static bool All<T>(this IEnumerable<T> values, Mono.Func<T, bool> predicate)
        {            
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in values)
            {
                if (!predicate(value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
#endif