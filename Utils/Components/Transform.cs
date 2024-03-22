using System.Numerics;
namespace Utils.Components;

public sealed class Transform(Vector3 scale, Quaternion rotation, Vector3 position)
{
    public static Transform Identity => new(Vector3.One, Quaternion.Identity, Vector3.Zero);

    public Transform() : this(Vector3.One, Quaternion.Identity, Vector3.Zero)
    {}

    public Vector3 Scale { get; set; } = scale;
    public Quaternion Rotation { get; set; } = rotation;
    public Vector3 Position { get; set; } = position;

    public Matrix4x4 WorldMatrix => Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);
    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    public void Translate(Vector3 translation)
    {
        Position += translation;
    }

    public void Rotate(Quaternion rotation)
    {
        Rotation *= rotation;
    }

    public void RotateAround(Vector3 target, Vector3 axis, float angle)
    {
        Vector3 relativePosition = Position - target;
        var rotation = Quaternion.CreateFromAxisAngle(axis, angle);
        relativePosition = Vector3.Transform(relativePosition, rotation);
        Position = target + relativePosition;
    }

    public void LookAt(Vector3 target, Vector3 up)
    {
        Vector3 forward = Vector3.Normalize(target - Position);
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
        Vector3 newUp = Vector3.Cross(forward, right);
        Rotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateWorld(Position, forward, newUp));
    }
}
