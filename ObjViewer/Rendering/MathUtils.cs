using System;
using System.Numerics;
using System.Runtime.Intrinsics;
namespace ObjViewer.Rendering;

public static class MathUtils
{
    public static Vector3 AsVector3(this Vector4 position) => position.AsVector128().AsVector3();
    public static Vector2 AsVector2(this Vector4 position) => position.AsVector128().AsVector2();
    public static float Cross(in Vector2 p1, in Vector2 p2) => p1.X * p2.Y - p1.Y * p2.X;
    public static float Clamp01(float value) => MathF.Max(0f, MathF.Min(1f, value));
    public static Vector2 PerspectiveCorrectLerp(Vector2 min, Vector2 max, float zMin, float zMax, float amount) =>
        Vector2.Lerp(min / zMin, max / zMax, amount) / float.Lerp(1f / zMin, 1f / zMax, amount);
}
