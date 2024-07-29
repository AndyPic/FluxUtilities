using System;

namespace Flux.Core.Extensions
{
    public static class StringExtentions
    {
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrEmpty(this string a)
        {
            return string.IsNullOrEmpty(a);
        }
    }
}