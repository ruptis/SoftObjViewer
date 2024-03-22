using System.Numerics;
using Utils;
using Utils.Components;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class VertexLinearInterpolator : ILinearInterpolator<Vertex>
{
    public Vertex Interpolate(in Vertex v0, in Vertex v1, float t) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, t),
        Normal = Vector3.Normalize(Vector3.Lerp(v0.Normal, v1.Normal, t)),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates, v1.TextureCoordinates, t),
        Tangent = Vector3.Normalize(Vector3.Lerp(v0.Tangent, v1.Tangent, t)),
    };
}
