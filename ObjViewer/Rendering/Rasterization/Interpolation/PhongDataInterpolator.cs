using System.Numerics;
using ObjViewer.Rendering.Shaders.Phong;
namespace ObjViewer.Rendering.Rasterization.Interpolation;

public class PhongDataInterpolator : IInterpolator<PhongShaderData>
{

    public PhongShaderData Average(in PhongShaderData v0, in PhongShaderData v1, in PhongShaderData v2) => new()
    {
        LightDirection = (v0.LightDirection + v1.LightDirection + v2.LightDirection) / 3,
        ViewDirection = (v0.ViewDirection + v1.ViewDirection + v2.ViewDirection) / 3,
        TextureCoordinates = (v0.TextureCoordinates + v1.TextureCoordinates + v2.TextureCoordinates) / 3,
    };

    public PhongShaderData Interpolate(in PhongShaderData v0, in PhongShaderData v1, float w0, float w1, float w, float amount) => new()
    {
        LightDirection = Vector3.Lerp(v0.LightDirection, v1.LightDirection, amount),
        ViewDirection = Vector3.Lerp(v0.ViewDirection, v1.ViewDirection, amount),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates * w0, v1.TextureCoordinates * w1, amount) / w,
    };
}
