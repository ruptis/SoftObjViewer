using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.Phong;

public readonly record struct PhongShaderInput(Vector3 LightDirection, Vector3 ViewDirection, Vector2 TextureCoordinates);
