using System.Numerics;
using GraphicsPipeline.Components.Shaders.MultiLightPhong;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class MultiLightPhongLinearInterpolator : ILinearInterpolator<MultiLightPhongShaderInput>
{
    public MultiLightPhongShaderInput Interpolate(in MultiLightPhongShaderInput v0, in MultiLightPhongShaderInput v1, float t) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, t),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, t),
        Tangent = Vector3.Lerp(v0.Tangent, v1.Tangent, t),
        Bitangent = Vector3.Lerp(v0.Bitangent, v1.Bitangent, t),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates, v1.TextureCoordinates, t)
    };
}
