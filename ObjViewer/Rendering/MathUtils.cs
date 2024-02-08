using System;
using System.Numerics;
using System.Runtime.Intrinsics;
namespace ObjViewer.Rendering;

public static class MathUtils
{
    public static Vector3 AsVector3(this Vector4 position) => position.AsVector128().AsVector3();
    public static Vector2 AsVector2(this Vector3 position) => position.AsVector128().AsVector2();
    public static float Cross(in Vector2 p1, in Vector2 p2) => p1.X * p2.Y - p1.Y * p2.X;
    public static float Clamp01(float value) => MathF.Max(0f, MathF.Min(1f, value));
    public static float Lerp(float min, float max, float gradient) => min + (max - min) * Clamp01(gradient);
    public static Vector2 Lerp(Vector2 min, Vector2 max, float gradient) => min + (max - min) * Clamp01(gradient);
    public static Vector3 Lerp(Vector3 min, Vector3 max, float gradient) => min + (max - min) * Clamp01(gradient);
    public static Vertex Lerp(Vertex min, Vertex max, float gradient) => new()
    {
        Position = Lerp(min.Position, max.Position, gradient),
        Normal = Lerp(min.Normal, max.Normal, gradient),
        TextureCoordinates = Lerp(min.TextureCoordinates, max.TextureCoordinates, gradient),
    };
}
