using System.Numerics;
namespace ObjViewer.Rendering;

public record struct Vertex(Vector3 Position, Vector3 Normal, Vector2 TextureCoordinate);
