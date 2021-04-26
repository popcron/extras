using UnityEngine;

public partial class Utils
{
    private static Mesh quadMesh;

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
}
