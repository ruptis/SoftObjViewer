using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class BresenhamRasterizer : IRasterizer<Vertex>
{
    private int _height;

    private Matrix4x4 _screenSpaceTransform;
    private int _width;

    public void SetViewport(int width, int height)
    {
        _width = width;
        _height = height;
        _screenSpaceTransform = Matrix4x4.CreateScale(width / 2f, -height / 2f, 1) * Matrix4x4.CreateTranslation(width / 2f, height / 2f, 0);
    }

    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        Vector3 screenSpaceA = ToScreenSpace(triangle.A);
        Vector3 screenSpaceB = ToScreenSpace(triangle.B);
        Vector3 screenSpaceC = ToScreenSpace(triangle.C);
        var xA = (int)screenSpaceA.X;
        var yA = (int)screenSpaceA.Y;
        var xB = (int)screenSpaceB.X;
        var yB = (int)screenSpaceB.Y;
        var xC = (int)screenSpaceC.X;
        var yC = (int)screenSpaceC.Y;

        DrawLine(xA, xB, yA, yB, screenSpaceA.Z, screenSpaceB.Z, triangle.AData, triangle.BData, fragmentCallback);
        DrawLine(xB, xC, yB, yC, screenSpaceB.Z, screenSpaceC.Z, triangle.BData, triangle.CData, fragmentCallback);
        DrawLine(xC, xA, yC, yA, screenSpaceC.Z, screenSpaceA.Z, triangle.CData, triangle.AData, fragmentCallback);
    }

    private void DrawLine(int x0, int x1, int y0, int y1, float z0, float z1, in Vertex v0, in Vertex v1, Action<Fragment<Vertex>> callback)
    {
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            if (IsInside(x0, y0))
            {
                callback(new Fragment<Vertex>
                {
                    Position = new Vector3(x0, y0, (z0 + z1) / 2),
                    Data = Interpolate(v0, v1, x0, y0, x1, y1)
                });
            }

            if (x0 == x1 && y0 == y1)
                break;
            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private Vertex Interpolate(in Vertex v0, in Vertex v1, int x0, int y0, int x1, int y1)
    {
        if (x0 == x1 || y0 == y1)
            return v0;

        var w0 = (x1 - x0) * (y1 - y0);
        var w1 = (x0 - x1) * (y1 - y0);
        var w2 = (x0 - x1) * (y0 - y1);
        var w = 1.0f / ((x1 - x0) * (y1 - y0));
        return new Vertex
        {
            Position = new Vector3(w * (v0.Position.X * w0 + v1.Position.X * w1 + v1.Position.X * w2),
                w * (v0.Position.Y * w0 + v1.Position.Y * w1 + v1.Position.Y * w2),
                w * (v0.Position.Z * w0 + v1.Position.Z * w1 + v1.Position.Z * w2)),
            Normal = Vector3.Normalize(w * (v0.Normal * w0 + v1.Normal * w1 + v1.Normal * w2)),
            TextureCoordinates = new Vector2(w * (v0.TextureCoordinates.X * w0 + v1.TextureCoordinates.X * w1 + v1.TextureCoordinates.X * w2),
                w * (v0.TextureCoordinates.Y * w0 + v1.TextureCoordinates.Y * w1 + v1.TextureCoordinates.Y * w2))
        };
    }

    private bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;

    private Vector3 ToScreenSpace(in Vector4 position) => ToVector3(Vector4.Transform(position, _screenSpaceTransform));
    private static Vector3 ToVector3(in Vector4 position) => new(position.X, position.Y, position.Z);
}
