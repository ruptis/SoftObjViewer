using System.Numerics;
using GraphicsPipeline.Components.Shaders.Phong;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class PhongLinearInterpolator : ILinearInterpolator<PhongShaderInput>
{
    public PhongShaderInput Interpolate(in PhongShaderInput v0, in PhongShaderInput v1, float t) => new()
    {
        LightDirection = Vector3.Lerp(v0.LightDirection, v1.LightDirection, t),
        ViewDirection = Vector3.Lerp(v0.ViewDirection, v1.ViewDirection, t),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates, v1.TextureCoordinates, t)
    };
}
