using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class BresenhamRasterizer : IRasterizer<Vertex>
{
    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        DrawLine(triangle.A, triangle.B, triangle.AData, triangle.BData, fragmentCallback);
        DrawLine(triangle.B, triangle.C, triangle.BData, triangle.CData, fragmentCallback);
        DrawLine(triangle.C, triangle.A, triangle.CData, triangle.AData, fragmentCallback);
    }

    private static void DrawLine(in Vector4 p0, in Vector4 p1, in Vertex v0, in Vertex v1, Action<Fragment<Vertex>> callback)
    {
        var x0 = (int)p0.X;
        var y0 = (int)p0.Y;
        var x1 = (int)p1.X;
        var y1 = (int)p1.Y;
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = Math.Sign(x1 - x0);
        var sy = Math.Sign(y1 - y0);
        var error = dx - dy;

        while (true)
        {
            DrawFragment(x0, y0, p0, p1, v0, v1, callback);

            if (x0 == x1 && y0 == y1)
                break;
            var e2 = 2 * error;
            if (e2 >= -dy)
            {
                if (x0 == x1)
                    break;
                error -= dy;
                x0 += sx;
            }
            if (e2 <= dx)
            {
                if (y0 == y1)
                    break;
                error += dx;
                y0 += sy;
            }
        }
    }

    private static void DrawFragment(int x, int y, in Vector4 p0, in Vector4 p1, in Vertex v0, in Vertex v1, Action<Fragment<Vertex>> callback)
    {
        var gradient = Math.Abs(p0.X - p1.X) > 0.0f ? MathUtils.Clamp01((x - p0.X) / (p1.X - p0.X)) : 1.0f;
        callback(new Fragment<Vertex>
        {
            Position = new Vector4(x, y, float.Lerp(p0.Z, p1.Z, gradient), float.Lerp(p0.W, p1.W, gradient)),
            Data = Interpolate(v0, v1, gradient)
        });
    }

    private static Vertex Interpolate(in Vertex v0, in Vertex v1, float gradient) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, gradient),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, gradient),
    };
}
