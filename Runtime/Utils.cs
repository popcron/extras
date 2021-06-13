using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public partial class Utils
{
    private static Mesh quadMesh;
    private static Mesh cubeMesh;
    private static Mesh sphereMesh;
    private static Random random = new Random();
    private static string[] buildAnim = new string[]{"⢀⠀",
    "⡀⠀",
    "⠄⠀",
    "⢂⠀",
    "⡂⠀",
    "⠅⠀",
    "⢃⠀",
    "⡃⠀",
    "⠍⠀",
    "⢋⠀",
    "⡋⠀",
    "⠍⠁",
    "⢋⠁",
    "⡋⠁",
    "⠍⠉",
    "⠋⠉",
    "⠋⠉",
    "⠉⠙",
    "⠉⠙",
    "⠉⠩",
    "⠈⢙",
    "⠈⡙",
    "⢈⠩",
    "⡀⢙",
    "⠄⡙",
    "⢂⠩",
    "⡂⢘",
    "⠅⡘",
    "⢃⠨",
    "⡃⢐",
    "⠍⡐",
    "⢋⠠",
    "⡋⢀",
    "⠍⡁",
    "⢋⠁",
    "⡋⠁",
    "⠍⠉",
    "⠋⠉",
    "⠋⠉",
    "⠉⠙",
    "⠉⠙",
    "⠉⠩",
    "⠈⢙",
    "⠈⡙",
    "⠈⠩",
    "⠀⢙",
    "⠀⡙",
    "⠀⠩",
    "⠀⢘",
    "⠀⡘",
    "⠀⠨",
    "⠀⢐",
    "⠀⡐",
    "⠀⠠",
    "⠀⢀",
    "⠀⡀"
    };

    public static Mesh QuadMesh
    {
        get
        {
            if (!quadMesh)
            {
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quadMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
                Object.DestroyImmediate(primitive);
            }

            return quadMesh;
        }
    }

    public static Mesh CubeMesh
    {
        get
        {
            if (!cubeMesh)
            {
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
                Object.DestroyImmediate(primitive);
            }

            return cubeMesh;
        }
    }

    public static Mesh SphereMesh
    {
        get
        {
            if (!sphereMesh)
            {
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
                Object.DestroyImmediate(primitive);
            }

            return sphereMesh;
        }
    }

    public static float PerlinRandom(int id)
    {
        id = Mathf.Abs(id);
        float time = Time.time * 25f;
        return (Mathf.PerlinNoise(time + id * 0.1f, id * 0.1f) - 0.5f) * 2f;
    }

    public static string GetLoadingText(float time)
    {
        time = Mathf.Abs(time * 17f);
        int index = Mathf.Clamp(Mathf.FloorToInt(time % buildAnim.Length), 0, buildAnim.Length - 1);
        return buildAnim[index];
    }

    public static string RandomString(int length, int? seed = null)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random r = random;
        if (seed != null)
        {
            r = new Random(seed.Value);
        }

        return new string(Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Returns true if A equals to B without case sensitivity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatch(string a, string b)
    {
        bool aNull = a is null;
        bool bNull = b is null;
        if (aNull && bNull)
        {
            return true;
        }
        else if (aNull != bNull)
        {
            return false;
        }

        return a.Equals(b, StringComparison.OrdinalIgnoreCase);
    }
}
