using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.Pbr;

public readonly record struct PbrShaderInput(
    Vector3 Position,
    Vector3 Normal,
    Vector3 Tangent,
    Vector3 Bitangent,
    Vector2 TextureCoordinates);



