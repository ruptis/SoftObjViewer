using System.Numerics;
using GraphicsPipeline.Components.Shaders.Phong;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class PhongScanlineInterpolator : IScanlineInterpolator<PhongShaderInput>
{
    public PhongShaderInput Average(in PhongShaderInput v0, in PhongShaderInput v1, in PhongShaderInput v2) => new()
    {
        LightDirection = (v0.LightDirection + v1.LightDirection + v2.LightDirection) / 3,
        ViewDirection = (v0.ViewDirection + v1.ViewDirection + v2.ViewDirection) / 3,
        TextureCoordinates = (v0.TextureCoordinates + v1.TextureCoordinates + v2.TextureCoordinates) / 3
    };

    public PhongShaderInput Interpolate(in PhongShaderInput v0, in PhongShaderInput v1, float w0, float w1, float w, float amount) => new()
    {
        LightDirection = Vector3.Lerp(v0.LightDirection, v1.LightDirection, amount),
        ViewDirection = Vector3.Lerp(v0.ViewDirection, v1.ViewDirection, amount),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates * w0, v1.TextureCoordinates * w1, amount) / w,
    };
}
