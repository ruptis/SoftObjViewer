namespace GraphicsPipeline;

public interface IRasterizer<T> where T : struct
{
    public void Rasterize(in Triangle<T> triangle, Action<Fragment<T>> fragmentCallback);
}
