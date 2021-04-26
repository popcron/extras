using System.Runtime.CompilerServices;
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