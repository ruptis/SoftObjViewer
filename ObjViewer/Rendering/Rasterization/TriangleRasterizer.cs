using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public sealed class TriangleRasterizer : IRasterizer<Vertex>
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
        /*if (CullTriangle(triangle))
            return;*/

        Vector3 screenSpaceA = ToScreenSpace(triangle.A);
        Vector3 screenSpaceB = ToScreenSpace(triangle.B);
        Vector3 screenSpaceC = ToScreenSpace(triangle.C);

        Vertex triangleAData = triangle.AData;
        Vertex triangleBData = triangle.BData;
        Vertex triangleCData = triangle.CData;

        DrawTriangle(ref screenSpaceA, ref screenSpaceB, ref screenSpaceC, ref triangleAData, ref triangleBData, ref triangleCData, fragmentCallback);
    }

    private static bool CullTriangle(in Triangle<Vertex> triangle)
    {
        Vector3 normal = Vector3.Cross(ToVector3(triangle.B - triangle.A), ToVector3(triangle.C - triangle.A));
        return Vector3.Dot(normal, ToVector3(triangle.A)) < 0;
    }

    private void DrawTriangle(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vertex v1, ref Vertex v2, ref Vertex v3, Action<Fragment<Vertex>> callback)
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

        var invSlope1 = p2.Y - p1.Y > 0 ? (p2.X - p1.X) / (p2.Y - p1.Y) : 0;
        var invSlope2 = p3.Y - p1.Y > 0 ? (p3.X - p1.X) / (p3.Y - p1.Y) : 0;

        if (invSlope1 > invSlope2)
            for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                if (y < p2.Y)
                    ProcessScanLine(y, ref p1, ref p3, ref p1, ref p2, ref v1, ref v3, ref v1, ref v2, callback);
                else
                    ProcessScanLine(y, ref p1, ref p3, ref p2, ref p3, ref v1, ref v3, ref v2, ref v3, callback);
        else
            for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                if (y < p2.Y)
                    ProcessScanLine(y, ref p1, ref p2, ref p1, ref p3, ref v1, ref v2, ref v1, ref v3, callback);
                else
                    ProcessScanLine(y, ref p2, ref p3, ref p1, ref p3, ref v2, ref v3, ref v1, ref v3, callback);
    }
    private void ProcessScanLine(int y, ref Vector3 pa, ref Vector3 pb, ref Vector3 pc, ref Vector3 pd, ref Vertex va, ref Vertex vb, ref Vertex vc, ref Vertex vd, Action<Fragment<Vertex>> callback)
    {
        var gradient1 = Math.Abs(pb.Y - pa.Y) > 0 ? (y - pa.Y) / (pb.Y - pa.Y) : 1;
        var gradient2 = Math.Abs(pd.Y - pc.Y) > 0 ? (y - pc.Y) / (pd.Y - pc.Y) : 1;

        var sx = (int)Lerp(pa.X, pb.X, gradient1);
        var ex = (int)Lerp(pc.X, pd.X, gradient2);

        var z1 = Lerp(pa.Z, pb.Z, gradient1);
        var z2 = Lerp(pc.Z, pd.Z, gradient2);

        Vertex v1 = Lerp(va, vb, gradient1);
        Vertex v2 = Lerp(vc, vd, gradient2);

        for (var x = sx; x < ex; x++)
        {
            var gradient = (x - sx) / (float)(ex - sx);
            var z = Lerp(z1, z2, gradient);
            Vertex v = Lerp(v1, v2, gradient);
            if (IsInside(x, y))
                callback(new Fragment<Vertex>
                {
                    Position = new Vector3(x, y, z),
                    Data = v
                });
        }
    }

    private bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    private Vector3 ToScreenSpace(in Vector4 position) => ToVector3(Vector4.Transform(position, _screenSpaceTransform));
    private static Vector3 ToVector3(in Vector4 position) => position.AsVector128().AsVector3();
    private static float Clamp01(float value) => MathF.Max(0, MathF.Min(1, value));
    private static float Lerp(float min, float max, float gradient) => min + (max - min) * Clamp01(gradient);
    private static Vector2 Lerp(Vector2 min, Vector2 max, float gradient) => min + (max - min) * Clamp01(gradient);
    private static Vector3 Lerp(Vector3 min, Vector3 max, float gradient) => min + (max - min) * Clamp01(gradient);
    private static Vertex Lerp(Vertex min, Vertex max, float gradient) => new()
    {
        Position = Lerp(min.Position, max.Position, gradient),
        Normal = Lerp(min.Normal, max.Normal, gradient),
        TextureCoordinates = Lerp(min.TextureCoordinates, max.TextureCoordinates, gradient)
    };
}
