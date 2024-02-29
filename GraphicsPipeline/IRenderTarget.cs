using System.Drawing;
namespace GraphicsPipeline;

public interface IRenderTarget
{
    public int Width { get; }
    public int Height { get; }
    public void SetPixel(float x, float y, float z, in Color color);
    public bool ZTest(float x, float y, float z);
}
