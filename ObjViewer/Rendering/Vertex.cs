using System.Numerics;
namespace ObjViewer.Rendering;

public readonly record struct Vertex(Vector3 Position, Vector3 Normal, Vector2 TextureCoordinates);
