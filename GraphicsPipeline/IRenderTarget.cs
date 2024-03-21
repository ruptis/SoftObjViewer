using System.Numerics;
namespace GraphicsPipeline;

public interface IRenderTarget
{
    public int Width { get; }
    public int Height { get; }
    public void SetPixel(float x, float y, in Vector4 color);
    public bool DepthTestAndLock(float x, float y, float z);
    public void UnlockPixel(float x, float y);
    public void Clear(in Vector4 color);
}
