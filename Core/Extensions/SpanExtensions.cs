using System;

public static class SpanExtensions
{
    public static bool Contains<T>(this Span<T> span, T target)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i].Equals(target))
                return true;
        }
        return false;
    }

    public static string ToStringExt<T>(this Span<T> span)
    {
        System.Text.StringBuilder sb = new();
        sb.Append('[');
        for (int i = 0; i < span.Length; i++)
        {
            sb.Append($"{span[i]}");
        }
        sb.Append(']');
        return sb.ToString();
    }
}