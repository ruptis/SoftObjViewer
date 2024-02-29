using System.Numerics;
namespace Utils;

public class Camera(float fieldOfView, float nearPlane, float farPlane, float aspectRatio)
{
    public Camera(float aspectRatio) : this(MathF.PI / 4, 0.1f, 100f, aspectRatio)
    {}

    public float FieldOfView { get; set; } = fieldOfView;
    public float NearPlane { get; set; } = nearPlane;
    public float FarPlane { get; set; } = farPlane;
    public float AspectRatio { get; set; } = aspectRatio;

    public Transform Transform { get; set; } = new();

    public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookTo(Transform.Position, -Transform.Forward, Transform.Up);
    public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
}
