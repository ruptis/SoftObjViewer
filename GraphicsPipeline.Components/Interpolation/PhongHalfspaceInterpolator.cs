using System.Numerics;
using GraphicsPipeline.Components.Shaders.Phong;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class PhongHalfspaceInterpolator : IHalfspaceInterpolator<PhongShaderInput>
{
    public PhongShaderInput Interpolate(in Triangle<PhongShaderInput> triangle, in Vector3 barycentric)
    {
        var a = barycentric.X * triangle.A.W;
        var b = barycentric.Y * triangle.B.W;
        var c = barycentric.Z * triangle.C.W;
        return new PhongShaderInput
        {
            LightDirection = triangle.AData.LightDirection * barycentric.X + triangle.BData.LightDirection * barycentric.Y + triangle.CData.LightDirection * barycentric.Z,
            ViewDirection = triangle.AData.ViewDirection * barycentric.X + triangle.BData.ViewDirection * barycentric.Y + triangle.CData.ViewDirection * barycentric.Z,
            TextureCoordinates = (triangle.AData.TextureCoordinates * a + triangle.BData.TextureCoordinates * b + triangle.CData.TextureCoordinates * c) /
                (a + b + c)
        };
    }
}
