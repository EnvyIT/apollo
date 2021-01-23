using System;

namespace Apollo.Util
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string current, string other)
        {
            return string.Equals(current, other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
