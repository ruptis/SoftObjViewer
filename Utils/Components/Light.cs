using System.Numerics;
namespace Utils.Components;

public enum LightType
{
    Directional,
    Point,
    Spot
}
public struct Light(
    Vector3 color,
    LightType type,
    float intensity,
    float range,
    float spotAngle)
{
    public Vector3 Color { get; set; } = color;
    public LightType Type { get; set; } = type;
    public float Intensity { get; set; } = intensity;
    public float Range
    {
        get => range;
        set
        {
            range = value;
            InvRange = 1.0f / range;
        }
    }
    public float InvRange { get; private set; } = 1.0f / range;
    public float SpotAngle { get; set; } = spotAngle;
}
