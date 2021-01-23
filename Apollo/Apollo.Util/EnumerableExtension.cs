using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;

namespace Apollo.Util
{
    public static class EnumerableExtension
    {
        public static string JoinKeyValuePairs<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs,
            string seed = "?", string delimiter = "&", string keyValueSeparator = "=")
        {
            var result =
                keyValuePairs?.Aggregate(seed, (current, next) => current + $"{DetermineKeyEmptyOrNull(next.Key, keyValueSeparator)}{next.Value}{delimiter}") ??
                Empty;
            return result == Empty ? result : result.Remove(result.Length - 1);
        }

        private static string DetermineKeyEmptyOrNull<TKey>(TKey key, string keyValueSeparator)
        {
            return key == null ? Empty : $"{key}{keyValueSeparator}";
        }

        public static string JoinValues<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs,
            string delimiter = "/")
        {
            var result = keyValuePairs?.Aggregate(Empty, (current, next) => current + $"{next.Value}{delimiter}") ??
                         Empty;
            return result == Empty ? result : result.Remove(result.Length - 1);
        }

        public static string ToPrettyString<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null
                ? Empty
                : Join(
                    Environment.NewLine,
                    enumerable.Select((property, index) => $"[{index}] {property.ToPrettyString()}")
                );
        }
    }
}