using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class BresenhamRasterizer : IRasterizer<Vertex>
{
    private int _height;
    private int _width;

    private Matrix4x4 _screenSpaceTransform;

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
        var error = dx - dy;

        while (true)
        {
            if (IsInside(x0, y0))
            {
                var gradient = x1 - x0 != 0 ? (float)(x0 - x1) / (x0 - x1) : 1;
                callback(new Fragment<Vertex>
                {
                    Position = new Vector3(x0, y0, float.Lerp(z0, z1, gradient)),
                    Data = Interpolate(v0, v1, gradient)
                });
            }

            if (x0 == x1 && y0 == y1)
                break;
            var e2 = 2 * error;
            if (e2 > -dy)
            {
                error -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                error += dx;
                y0 += sy;
            }
        }
    }
    
    private bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;

    private Vector3 ToScreenSpace(in Vector4 position) => Vector4.Transform(position, _screenSpaceTransform).AsVector3();
    private static Vertex Interpolate(in Vertex v0, in Vertex v1, float gradient) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, gradient),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, gradient),
    };
}
