namespace GraphicsPipeline.Components.Rasterization.Interpolation;

public interface IScanlineInterpolator<T> where T : struct
{
    public T Average(in T v0, in T v1, in T v2);
    public T Interpolate(in T v0, in T v1, float w0, float w1, float w, float amount);
}