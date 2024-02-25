using System.Numerics;
namespace GraphicsPipeline.Components;

public readonly record struct Vertex(Vector3 Position, Vector3 Normal, Vector2 TextureCoordinates, Vector3 Tangent);
