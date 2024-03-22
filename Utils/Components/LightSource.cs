using System.Numerics;
namespace Utils.Components;

public class LightSource(Transform transform, Light light)
{
    public Transform Transform { get; init; } = transform;
    public BoundingBox BoundingBox { get; init; } = BoundingBox.FromTransform(transform, Vector3.One);
    public Light Light { get; init; } = light;
}