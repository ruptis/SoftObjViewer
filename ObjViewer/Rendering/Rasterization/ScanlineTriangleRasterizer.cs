using System;
using System.Numerics;
using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization.Interpolation;
namespace ObjViewer.Rendering.Rasterization;

public sealed class ScanlineTriangleRasterizer<T, TInterpolator> : IRasterizer<T> 
    where T : struct 
    where TInterpolator : IInterpolator<T>, new()
{
    private readonly IInterpolator<T> _interpolator = new TInterpolator();
    
    public bool InterpolationEnabled { get; set; } = true;

    public void Rasterize(in Triangle<T> triangle, Action<Fragment<T>> fragmentCallback)
    {
        Vector4 triangleA = triangle.A;
        Vector4 triangleB = triangle.B;
        Vector4 triangleC = triangle.C;

        T triangleAData = triangle.AData;
        T triangleBData = triangle.BData;
        T triangleCData = triangle.CData;

        T average = InterpolationEnabled ? default : _interpolator.Average(in triangleAData, in triangleBData, in triangleCData);
        DrawTriangle(ref triangleA, ref triangleB, ref triangleC, ref triangleAData, ref triangleBData, ref triangleCData, fragmentCallback, in average);
    }

    private void DrawTriangle(ref Vector4 p1, ref Vector4 p2, ref Vector4 p3, ref T v1, ref T v2, ref T v3, Action<Fragment<T>> callback, in T average)
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


    private void ProcessScanLine(int y, in Vector4 pa, in Vector4 pb, in Vector4 pc, in Vector4 pd, in T va, in T vb, in T vc, in T vd, Action<Fragment<T>> callback, in T average)
    {
        var gradient1 = MathF.Abs(pb.Y - pa.Y) > 0f ? MathUtils.Clamp01((y - pa.Y) / (pb.Y - pa.Y)) : 1f;
        var gradient2 = MathF.Abs(pd.Y - pc.Y) > 0f ? MathUtils.Clamp01((y - pc.Y) / (pd.Y - pc.Y)) : 1f;

        var sx = (int)float.Lerp(pa.X, pb.X, gradient1);
        var ex = (int)float.Lerp(pc.X, pd.X, gradient2);

        var z1 = float.Lerp(pa.Z, pb.Z, gradient1);
        var z2 = float.Lerp(pc.Z, pd.Z, gradient2);

        var w1 = float.Lerp(pa.W, pb.W, gradient1);
        var w2 = float.Lerp(pc.W, pd.W, gradient2);

        T v1 = InterpolationEnabled ? _interpolator.Interpolate(va, vb, pa.W, pb.W, w1, gradient1) : average;
        T v2 = InterpolationEnabled ? _interpolator.Interpolate(vc, vd, pc.W, pd.W, w2, gradient2) : average;

        for (var x = sx; x < ex; x++)
        {
            var gradient = (x - sx) / (float)(ex - sx);
            var z = float.Lerp(z1, z2, gradient);
            var w = float.Lerp(w1, w2, gradient);
            T v = InterpolationEnabled ? _interpolator.Interpolate(v1, v2, w1, w2, w, gradient) : average;
            callback(new Fragment<T>
            {
                Position = new Vector4(x, y, z, w),
                Data = v
            });
        }
    }
}
