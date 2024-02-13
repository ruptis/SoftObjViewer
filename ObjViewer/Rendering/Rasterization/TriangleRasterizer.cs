using System;
using System.Diagnostics;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class TriangleRasterizer : IRasterizer<Vertex>
{
    private int _height;
    private int _width;

    private Matrix4x4 _viewport;

    public bool InterpolationEnabled { get; set; } = true;

    public void SetViewport(int width, int height)
    {
        if (_width == width && _height == height) return;
        _width = width;
        _height = height;
        _viewport = Matrix4x4.CreateViewportLeftHanded(0, 0, width, height, 0, 1);
    }

    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        Vector3 screenSpaceA = ToScreenSpace(triangle.A);
        Vector3 screenSpaceB = ToScreenSpace(triangle.B);
        Vector3 screenSpaceC = ToScreenSpace(triangle.C);

        Vertex triangleAData = triangle.AData;
        Vertex triangleBData = triangle.BData;
        Vertex triangleCData = triangle.CData;

        Vertex average = InterpolationEnabled ? default : new Vertex
        {
            Position = (triangleAData.Position + triangleBData.Position + triangleCData.Position) / 3,
            Normal = (triangleAData.Normal + triangleBData.Normal + triangleCData.Normal) / 3,
            TextureCoordinates = (triangleAData.TextureCoordinates + triangleBData.TextureCoordinates + triangleCData.TextureCoordinates) / 3
        };
        DrawTriangle(ref screenSpaceA, ref screenSpaceB, ref screenSpaceC, ref triangleAData, ref triangleBData, ref triangleCData, fragmentCallback, ref average);
    }

    private void DrawTriangle(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vertex v1, ref Vertex v2, ref Vertex v3, Action<Fragment<Vertex>> callback, ref Vertex average)
    {
        if (p1.Y > p2.Y)
        {
            (p1, p2) = (p2, p1);
            (v1, v2) = (v2, v1);
        }

        if (p2.Y > p3.Y)
        {
            (p2, p3) = (p3, p2);
            (v2, v3) = (v3, v2);
        }

        if (p1.Y > p2.Y)
        {
            (p1, p2) = (p2, p1);
            (v1, v2) = (v2, v1);
        }

        if (MathUtils.Cross((p2 - p1).AsVector2(), (p3 - p1).AsVector2()) >= 0f)
            for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
            {
                if (y < (int)p2.Y)
                    ProcessScanLine(y, ref p1, ref p3, ref p1, ref p2, ref v1, ref v3, ref v1, ref v2, callback, ref average);
                else
                    ProcessScanLine(y, ref p1, ref p3, ref p2, ref p3, ref v1, ref v3, ref v2, ref v3, callback, ref average);
            }
        else
            for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
            {
                if (y < (int)p2.Y)
                    ProcessScanLine(y, ref p1, ref p2, ref p1, ref p3, ref v1, ref v2, ref v1, ref v3, callback, ref average);
                else
                    ProcessScanLine(y, ref p2, ref p3, ref p1, ref p3, ref v2, ref v3, ref v1, ref v3, callback, ref average);
            }
    }


    private void ProcessScanLine(int y, ref Vector3 pa, ref Vector3 pb, ref Vector3 pc, ref Vector3 pd, ref Vertex va, ref Vertex vb, ref Vertex vc, ref Vertex vd, Action<Fragment<Vertex>> callback, ref Vertex average)
    {
        var gradient1 = MathF.Abs(pb.Y - pa.Y) > 0f ? MathUtils.Clamp01((y - pa.Y) / (pb.Y - pa.Y)) : 1f;
        var gradient2 = MathF.Abs(pd.Y - pc.Y) > 0f ? MathUtils.Clamp01((y - pc.Y) / (pd.Y - pc.Y)) : 1f;

        var sx = (int)float.Lerp(pa.X, pb.X, gradient1);
        var ex = (int)float.Lerp(pc.X, pd.X, gradient2);

        var z1 = float.Lerp(pa.Z, pb.Z, gradient1);
        var z2 = float.Lerp(pc.Z, pd.Z, gradient2);

        Vertex v1 = InterpolationEnabled ? Interpolate(va, vb, pa.Z, pb.Z, gradient1) : average;
        Vertex v2 = InterpolationEnabled ? Interpolate(vc, vd, pc.Z, pd.Z, gradient2) : average;

        for (var x = sx; x < ex; x++)
        {
            if (!IsInside(x, y)) continue;
            var gradient = (x - sx) / (float)(ex - sx);
            var z = float.Lerp(z1, z2, gradient);
            Vertex v = InterpolationEnabled ? Interpolate(v1, v2, z1, z2, gradient) : average;
            callback(new Fragment<Vertex>
            {
                Position = new Vector3(x, y, z),
                Data = v
            });
        }
    }

    private bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    private Vector3 ToScreenSpace(in Vector4 position) => Vector4.Transform(position, _viewport).AsVector3();

    private static Vertex Interpolate(in Vertex v0, in Vertex v1, float z0, float z1, float amount) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, amount),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, amount),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates, v1.TextureCoordinates, amount),
        W = float.Lerp(v0.W, v1.W, amount)
    };
}
