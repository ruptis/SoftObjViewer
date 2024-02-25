using System.Numerics;
namespace GraphicsPipeline.Components.Rasterization.Interpolation;

public interface IHalfspaceInterpolator<T> where T : struct
{
    public T Interpolate(in Triangle<T> triangle, in Vector3 barycentric);
}