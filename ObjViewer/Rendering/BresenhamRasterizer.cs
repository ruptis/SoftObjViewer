using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering;

public sealed class BresenhamRasterizer : IRasterizer<Vertex>
{
    private int _width;
    private int _height;

    public void SetViewport(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        var xA = ToScreenSpaceX(triangle.A.X);
        var yA = ToScreenSpaceY(triangle.A.Y);
        var xB = ToScreenSpaceX(triangle.B.X);
        var yB = ToScreenSpaceY(triangle.B.Y);
        var xC = ToScreenSpaceX(triangle.C.X);
        var yC = ToScreenSpaceY(triangle.C.Y);

        DrawLine(xA, xB, yA, yB, triangle.AData, triangle.BData, fragmentCallback);
        DrawLine(xB, xC, yB, yC, triangle.BData, triangle.CData, fragmentCallback);
        DrawLine(xC, xA, yC, yA, triangle.CData, triangle.AData, fragmentCallback);
    }

    private void DrawLine(int x0, int x1, int y0, int y1, in Vertex v0, in Vertex v1, Action<Fragment<Vertex>> callback)
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
                    Position = new Vector2(x0, y0),
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
        };
    }

    private bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    private int ToScreenSpaceX(float x) => (int)((x + 1) * _width / 2);
    private int ToScreenSpaceY(float y) => (int)((1 - y) * _height / 2);
}
