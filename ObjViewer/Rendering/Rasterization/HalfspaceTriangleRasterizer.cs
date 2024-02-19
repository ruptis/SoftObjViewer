using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class HalfspaceTriangleRasterizer : IRasterizer<Vertex>
{
    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        var minX = (int)MathF.Min(triangle.A.X, MathF.Min(triangle.B.X, triangle.C.X));
        var minY = (int)MathF.Min(triangle.A.Y, MathF.Min(triangle.B.Y, triangle.C.Y));
        var maxX = (int)MathF.Max(triangle.A.X, MathF.Max(triangle.B.X, triangle.C.X));
        var maxY = (int)MathF.Max(triangle.A.Y, MathF.Max(triangle.B.Y, triangle.C.Y));
        var area = EdgeFunction(triangle.A, triangle.B, triangle.C);

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var p = new Vector4(x, y, 0, 0);
                var w0 = EdgeFunction(triangle.B, triangle.C, p);
                var w1 = EdgeFunction(triangle.C, triangle.A, p);
                var w2 = EdgeFunction(triangle.A, triangle.B, p);

                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    var barycentric = new Vector3(w0 / area, w1 / area, w2 / area);
                    Vertex interpolated = Interpolate(triangle.AData, triangle.BData, triangle.CData, barycentric, triangle.A.W, triangle.B.W, triangle.C.W);
                    fragmentCallback(new Fragment<Vertex>
                    {
                        Position = new Vector4
                        {
                            X = x,
                            Y = y,
                            Z = triangle.A.Z * barycentric.X + triangle.B.Z * barycentric.Y + triangle.C.Z * barycentric.Z,
                            W = triangle.A.W * barycentric.X + triangle.B.W * barycentric.Y + triangle.C.W * barycentric.Z
                        },
                        Data = interpolated
                    });
                }
            }
        }
    }

    private static float EdgeFunction(in Vector4 a, in Vector4 b, in Vector4 c) => (c.X - a.X) * (b.Y - a.Y) - (c.Y - a.Y) * (b.X - a.X);
    private static Vertex Interpolate(in Vertex a, in Vertex b, in Vertex c, in Vector3 barycentric, float w0, float w1, float w2) =>
        new()
        {
            Position = a.Position * barycentric.X + b.Position * barycentric.Y + c.Position * barycentric.Z,
            Normal = a.Normal * barycentric.X + b.Normal * barycentric.Y + c.Normal * barycentric.Z,
            TextureCoordinates = (a.TextureCoordinates * w0 * barycentric.X + b.TextureCoordinates * w1 * barycentric.Y + c.TextureCoordinates * w2 * barycentric.Z) /
                (barycentric.X * w0 + barycentric.Y * w1 + barycentric.Z * w2)
        };
}
