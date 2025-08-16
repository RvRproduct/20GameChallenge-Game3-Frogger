using UnityEngine;

public static class VectorConversions
{
    public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 v)
    {
        return new UnityEngine.Vector2(v.X, v.Y);
    }

    public static System.Numerics.Vector2 ToSystem(this UnityEngine.Vector2 v)
    {
        return new System.Numerics.Vector2(v.x, v.y);
    }

    public static UnityEngine.Vector3 ToUnity(this System.Numerics.Vector3 v)
    {
        return new UnityEngine.Vector3(v.X, v.Y, v.Z);
    }

    public static System.Numerics.Vector3 ToSystem(this UnityEngine.Vector3 v)
    {
        return new System.Numerics.Vector3(v.x, v.y, v.z);
    }
}
