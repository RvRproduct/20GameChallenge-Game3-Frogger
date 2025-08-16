using UnityEngine;

public static class Vector2Conversions
{
    public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 v)
    {
        return new UnityEngine.Vector2(v.X, v.Y);
    }

    public static System.Numerics.Vector2 ToSystem(this UnityEngine.Vector2 v)
    {
        return new System.Numerics.Vector2(v.x, v.y);
    }
}
