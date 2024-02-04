using System.Drawing;
namespace GraphicsPipeline;

public interface IRenderTarget
{
    public int Width { get; }
    public int Height { get; }
    public void DrawPixel(float x, float y, float z, in Color color);
    public void Present();
    public void Clear(in Color color);
}