using System.Numerics;
using System.Runtime.Intrinsics;
namespace Utils;

public static class MathUtils
{
    public static Vector3 AsVector3(this Vector4 position) => position.AsVector128().AsVector3();
    public static Vector2 AsVector2(this Vector4 position) => position.AsVector128().AsVector2();
    public static float Cross(in Vector2 p1, in Vector2 p2) => p1.X * p2.Y - p1.Y * p2.X;
    public static float Clamp01(float value) => MathF.Max(0f, MathF.Min(1f, value));
    public static Vector2 PerspectiveCorrectLerp(Vector2 min, Vector2 max, float zMin, float zMax, float amount) =>
        Vector2.Lerp(min / zMin, max / zMax, amount) / float.Lerp(1f / zMin, 1f / zMax, amount);

    public static Vector3 Pow(this in Vector3 value, float power) => new(
        MathF.Pow(value.X, power),
        MathF.Pow(value.Y, power),
        MathF.Pow(value.Z, power)
    );
    
    public static Vector4 Pow(this in Vector4 value, float power) => new(
        MathF.Pow(value.X, power),
        MathF.Pow(value.Y, power),
        MathF.Pow(value.Z, power),
        MathF.Pow(value.W, power)
    );

    public static Matrix4x4 TbnSpace(in Vector3 tangent, in Vector3 bitangent, in Vector3 normal) => new(
        tangent.X, tangent.Y, tangent.Z, 0.0f,
        bitangent.X, bitangent.Y, bitangent.Z, 0.0f,
        normal.X, normal.Y, normal.Z, 0.0f,
        0.0f, 0.0f, 0.0f, 0.0f
    );
}
