using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class FloatExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVectorFromDegrees(this float f)
    {
        f *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVectorFromRadians(this float f)
    {
        return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this float f)
    {
        return Mathf.RoundToInt(f);
    }
}

public static class StringExtensions
{
    private static StringBuilder builder = new StringBuilder();

    public static string ToCamelCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        builder.Clear();
        builder.Append(text);
        builder[0] = char.ToLower(builder[0]);
        return builder.ToString();
    }

    public static string ToPascalCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        builder.Clear();
        builder.Append(text);
        builder[0] = char.ToUpper(builder[0]);
        return builder.ToString();
    }
}