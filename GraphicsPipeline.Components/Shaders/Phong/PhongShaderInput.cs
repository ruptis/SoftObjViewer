using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.Phong;

public readonly record struct PhongShaderInput(Vector3 Position, Vector3 Normal, Vector2 TextureCoordinates, Vector3 Tangent, Vector3 Bitangent);
