using System.Numerics;
namespace Utils;

public enum LightType
{
    Directional,
    Point,
    Spot
}
public class Light(
    Transform transform,
    Vector3 color,
    LightType type,
    float intensity,
    float range,
    float spotAngle)
{
    public Transform Transform { get; init; } = transform;
    public Vector3 Color { get; set; } = color;
    public LightType Type { get; set; } = type;
    public float Intensity { get; set; } = intensity;
    public float Range { get; set; } = range;
    public float SpotAngle { get; set; } = spotAngle;

    public static Light Default =>
        new(new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One),
            Vector3.One,
            LightType.Directional,
            1,
            10,
            45);
}
