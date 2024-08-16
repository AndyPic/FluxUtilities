using System;
using System.Text.RegularExpressions;

public static class EnumExtentions
{
    public static string ToStringExt<T>(this T value) where T : Enum
    {
        string enumName = value.ToString();
        string cleanedName = Regex.Replace(enumName, "(?<!^)([A-Z])", " $1");
        return cleanedName;
    }
}