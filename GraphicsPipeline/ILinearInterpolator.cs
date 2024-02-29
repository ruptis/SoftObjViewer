namespace GraphicsPipeline;

public interface ILinearInterpolator<T> where T : unmanaged
{
    public T Interpolate(in T v0, in T v1, float t);
}
