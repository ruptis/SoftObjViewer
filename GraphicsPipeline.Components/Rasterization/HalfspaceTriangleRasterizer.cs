using System.Numerics;
using GraphicsPipeline.Components.Rasterization.Interpolation;
namespace GraphicsPipeline.Components.Rasterization;

public sealed class HalfspaceTriangleRasterizer<T, TInterpolator> : IRasterizer<T>
    where T : struct
    where TInterpolator : IHalfspaceInterpolator<T>, new()
{
    private readonly TInterpolator _interpolator = new();

    public void Rasterize(in Triangle<T> triangle, Action<Fragment<T>> fragmentCallback)
    {
        var minX = (int)MathF.Min(triangle.A.X, MathF.Min(triangle.B.X, triangle.C.X));
        var minY = (int)MathF.Min(triangle.A.Y, MathF.Min(triangle.B.Y, triangle.C.Y));
        var maxX = (int)MathF.Max(triangle.A.X, MathF.Max(triangle.B.X, triangle.C.X));
        var maxY = (int)MathF.Max(triangle.A.Y, MathF.Max(triangle.B.Y, triangle.C.Y));
        
        Vector3 i = new(
            triangle.C.Y - triangle.B.Y,
            triangle.A.Y - triangle.C.Y,
            triangle.B.Y - triangle.A.Y);
        Vector3 j = new(
            triangle.C.X - triangle.B.X,
            triangle.A.X - triangle.C.X,
            triangle.B.X - triangle.A.X);
        Vector3 k = new(
            triangle.B.Y * triangle.C.X - triangle.B.X * triangle.C.Y,
            triangle.C.Y * triangle.A.X - triangle.C.X * triangle.A.Y,
            triangle.A.Y * triangle.B.X - triangle.A.X * triangle.B.Y);
        
        var area = TriangleArea(in triangle);
        
        Vector3 wY = i * minX - j * minY + k;

        for (var y = minY; y <= maxY; y++, wY -= j)
        {
            Vector3 wX = wY;
            for (var x = minX; x <= maxX; x++, wX += i)
            {
                if (wX is not { X: >= 0, Y: >= 0, Z: >= 0 })
                    continue;
                
                Vector3 barycentric = wX / area;
                var z = InterpolateZ(in triangle, in barycentric);
                var w = InterpolateW(in triangle, in barycentric);
                T interpolated = _interpolator.Interpolate(in triangle, in barycentric);
                fragmentCallback(new Fragment<T>
                {
                    Position = new Vector4(x, y, z, w),
                    Data = interpolated,
                });
            }
        }
    }
    
    private static float TriangleArea(in Triangle<T> triangle) =>
        (triangle.C.X - triangle.A.X) * (triangle.B.Y - triangle.A.Y) - (triangle.B.X - triangle.A.X) * (triangle.C.Y - triangle.A.Y);

    private static float InterpolateZ(in Triangle<T> triangle, in Vector3 barycentric) =>
        barycentric.X * triangle.A.Z + barycentric.Y * triangle.B.Z + barycentric.Z * triangle.C.Z;

    private static float InterpolateW(in Triangle<T> triangle, in Vector3 barycentric) =>
        barycentric.X * triangle.A.W + barycentric.Y * triangle.B.W + barycentric.Z * triangle.C.W;
}
