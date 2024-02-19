using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class ScanlineTriangleRasterizer : IRasterizer<Vertex>
{
    public bool InterpolationEnabled { get; set; } = true;

    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        Vector4 triangleA = triangle.A;
        Vector4 triangleB = triangle.B;
        Vector4 triangleC = triangle.C;

        Vertex triangleAData = triangle.AData;
        Vertex triangleBData = triangle.BData;
        Vertex triangleCData = triangle.CData;

        Vertex average = InterpolationEnabled ? default : new Vertex
        {
            Position = (triangleAData.Position + triangleBData.Position + triangleCData.Position) / 3,
            Normal = (triangleAData.Normal + triangleBData.Normal + triangleCData.Normal) / 3,
            TextureCoordinates = (triangleAData.TextureCoordinates + triangleBData.TextureCoordinates + triangleCData.TextureCoordinates) / 3
        };
        DrawTriangle(ref triangleA, ref triangleB, ref triangleC, ref triangleAData, ref triangleBData, ref triangleCData, fragmentCallback, in average);
    }

    private void DrawTriangle(ref Vector4 p1, ref Vector4 p2, ref Vector4 p3, ref Vertex v1, ref Vertex v2, ref Vertex v3, Action<Fragment<Vertex>> callback, in Vertex average)
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
                    ProcessScanLine(y, in p1, in p3, in p1, in p2, in v1, in v3, in v1, in v2, callback, in average);
                else
                    ProcessScanLine(y, in p1, in p3, in p2, in p3, in v1, in v3, in v2, in v3, callback, in average);
            }
        else
            for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
            {
                if (y < (int)p2.Y)
                    ProcessScanLine(y, in p1, in p2, in p1, in p3, in v1, in v2, in v1, in v3, callback, in average);
                else
                    ProcessScanLine(y, in p2, in p3, in p1, in p3, in v2, in v3, in v1, in v3, callback, in average);
            }
    }


    private void ProcessScanLine(int y, in Vector4 pa, in Vector4 pb, in Vector4 pc, in Vector4 pd, in Vertex va, in Vertex vb, in Vertex vc, in Vertex vd, Action<Fragment<Vertex>> callback, in Vertex average)
    {
        var gradient1 = MathF.Abs(pb.Y - pa.Y) > 0f ? MathUtils.Clamp01((y - pa.Y) / (pb.Y - pa.Y)) : 1f;
        var gradient2 = MathF.Abs(pd.Y - pc.Y) > 0f ? MathUtils.Clamp01((y - pc.Y) / (pd.Y - pc.Y)) : 1f;

        var sx = (int)float.Lerp(pa.X, pb.X, gradient1);
        var ex = (int)float.Lerp(pc.X, pd.X, gradient2);

        var z1 = float.Lerp(pa.Z, pb.Z, gradient1);
        var z2 = float.Lerp(pc.Z, pd.Z, gradient2);

        var w1 = float.Lerp(pa.W, pb.W, gradient1);
        var w2 = float.Lerp(pc.W, pd.W, gradient2);

        Vertex v1 = InterpolationEnabled ? Interpolate(va, vb, pa.W, pb.W, w1, gradient1) : average;
        Vertex v2 = InterpolationEnabled ? Interpolate(vc, vd, pc.W, pd.W, w2, gradient2) : average;

        for (var x = sx; x < ex; x++)
        {
            var gradient = (x - sx) / (float)(ex - sx);
            var z = float.Lerp(z1, z2, gradient);
            var w = float.Lerp(w1, w2, gradient);
            Vertex v = InterpolationEnabled ? Interpolate(v1, v2, w1, w2, w, gradient) : average;
            callback(new Fragment<Vertex>
            {
                Position = new Vector4(x, y, z, w),
                Data = v
            });
        }
    }

    private static Vertex Interpolate(in Vertex v0, in Vertex v1, float w0, float w1, float w, float amount) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, amount),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, amount),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates * w0, v1.TextureCoordinates * w1, amount) / w
    };
}
