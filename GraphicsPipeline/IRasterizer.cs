namespace GraphicsPipeline;

public interface IRasterizer<T> where T : struct
{
    public void SetViewport(int width, int height);
    public void Rasterize(in Triangle<T> triangle, Action<Fragment<T>> fragmentCallback);
}