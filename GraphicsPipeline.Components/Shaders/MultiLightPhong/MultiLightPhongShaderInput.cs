using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.MultiLightPhong;

public readonly record struct MultiLightPhongShaderInput(
    Vector3 Position, Vector3 Normal, Vector3 Tangent, Vector3 Bitangent, Vector2 TextureCoordinates);
