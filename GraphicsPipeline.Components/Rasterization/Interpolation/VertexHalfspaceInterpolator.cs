using System.Numerics;
namespace GraphicsPipeline.Components.Rasterization.Interpolation;

public sealed class VertexHalfspaceInterpolator : IHalfspaceInterpolator<Vertex>
{
    public Vertex Interpolate(in Triangle<Vertex> triangle, in Vector3 barycentric)
    {
        var a = barycentric.X * triangle.A.W;
        var b = barycentric.Y * triangle.B.W;
        var c = barycentric.Z * triangle.C.W;
        return new Vertex
        {
            Position = triangle.AData.Position * barycentric.X + triangle.BData.Position * barycentric.Y + triangle.CData.Position * barycentric.Z,
            Normal = triangle.AData.Normal * barycentric.X + triangle.BData.Normal * barycentric.Y + triangle.CData.Normal * barycentric.Z,
            TextureCoordinates = (triangle.AData.TextureCoordinates * a + triangle.BData.TextureCoordinates * b + triangle.CData.TextureCoordinates * c) /
                (a + b + c)
        };
    }
}
