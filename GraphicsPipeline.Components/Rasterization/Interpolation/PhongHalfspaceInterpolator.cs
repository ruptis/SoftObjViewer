using System.Numerics;
using GraphicsPipeline.Components.Shaders.Phong;
namespace GraphicsPipeline.Components.Rasterization.Interpolation;

public sealed class PhongHalfspaceInterpolator : IHalfspaceInterpolator<PhongShaderInput>
{
    public PhongShaderInput Interpolate(in Triangle<PhongShaderInput> triangle, in Vector3 barycentric)
    {
        var a = barycentric.X * triangle.A.W;
        var b = barycentric.Y * triangle.B.W;
        var c = barycentric.Z * triangle.C.W;
        return new PhongShaderInput
        {
            Position = triangle.AData.Position * barycentric.X + triangle.BData.Position * barycentric.Y + triangle.CData.Position * barycentric.Z,
            Normal = triangle.AData.Normal * barycentric.X + triangle.BData.Normal * barycentric.Y + triangle.CData.Normal * barycentric.Z,
            TextureCoordinates = (triangle.AData.TextureCoordinates * a + triangle.BData.TextureCoordinates * b + triangle.CData.TextureCoordinates * c) /
                (a + b + c),
            Tangent = triangle.AData.Tangent * barycentric.X + triangle.BData.Tangent * barycentric.Y + triangle.CData.Tangent * barycentric.Z,
            Bitangent = triangle.AData.Bitangent * barycentric.X + triangle.BData.Bitangent * barycentric.Y + triangle.CData.Bitangent * barycentric.Z
        };
    }
}
