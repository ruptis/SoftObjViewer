using System;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Rasterization;

public class TriangleRasterizer : IRasterizer<Vertex>
{
    private int _height;
    private int _width;

    private Matrix4x4 _screenSpaceTransform;

    public bool InterpolationEnabled { get; set; } = true;

    public void SetViewport(int width, int height)
    {
        _width = width;
        _height = height;
        _screenSpaceTransform = Matrix4x4.CreateScale(width / 2f, -height / 2f, 1) * Matrix4x4.CreateTranslation(width / 2f, height / 2f, 0);
    }

    public void Rasterize(in Triangle<Vertex> triangle, Action<Fragment<Vertex>> fragmentCallback)
    {
        if (CullTriangle(triangle))
            return;

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

    private static bool CullTriangle(in Triangle<Vertex> triangle)
    {
        Vector3 normal = Vector3.Cross((triangle.B - triangle.A).AsVector3(), (triangle.C - triangle.A).AsVector3());
        Vector3 viewDirection = (-triangle.A).AsVector3();
        return Vector3.Dot(normal, viewDirection) < 0;
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
        var gradient1 = MathF.Abs(pb.Y - pa.Y) > 0f ? (y - pa.Y) / (pb.Y - pa.Y) : 1f;
        var gradient2 = MathF.Abs(pd.Y - pc.Y) > 0f ? (y - pc.Y) / (pd.Y - pc.Y) : 1f;

        var sx = (int)MathUtils.Lerp(pa.X, pb.X, gradient1);
        var ex = (int)MathUtils.Lerp(pc.X, pd.X, gradient2);

        var z1 = MathUtils.Lerp(pa.Z, pb.Z, gradient1);
        var z2 = MathUtils.Lerp(pc.Z, pd.Z, gradient2);


        Vertex v1 = InterpolationEnabled ? MathUtils.Lerp(va, vb, gradient1) : average;
        Vertex v2 = InterpolationEnabled ? MathUtils.Lerp(vc, vd, gradient2) : average;

        for (var x = sx; x < ex; x++)
        {
            var gradient = (x - sx) / (float)(ex - sx);
            var z = MathUtils.Lerp(z1, z2, gradient);
            Vertex v = InterpolationEnabled ? MathUtils.Lerp(v1, v2, gradient) : average;
            if (IsInside(x, y))
                callback(new Fragment<Vertex>
                {
                    Position = new Vector3(x, y, z),
                    Data = v
                });
        }
    }

    private bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
    private Vector3 ToScreenSpace(in Vector4 position) => Vector4.Transform(position, _screenSpaceTransform).AsVector3();

}
