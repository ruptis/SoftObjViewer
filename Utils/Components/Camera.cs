using System.Numerics;
using Utils.Utils;
namespace Utils.Components;

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

    public Ray ScreenPointToRay(Vector2 screenPoint, Vector2 viewport)
    {
        var rayEndNdc = new Vector4(screenPoint.X / viewport.X * 2 - 1, 1 - screenPoint.Y / viewport.Y * 2, 1, 1);

        Matrix4x4.Invert(ViewMatrix, out Matrix4x4 invView);
        Matrix4x4.Invert(ProjectionMatrix, out Matrix4x4 invProj);

        Vector4 rayEndClip = Vector4.Transform(rayEndNdc, invProj);

        Vector4 rayEnd = Vector4.Transform(rayEndClip, invView);
        rayEnd /= rayEnd.W;

        return new Ray(Transform.Position, Vector3.Normalize(rayEnd.AsVector3() - Transform.Position));
    }
}
