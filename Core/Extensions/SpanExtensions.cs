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
}