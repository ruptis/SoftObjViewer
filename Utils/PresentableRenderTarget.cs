using System.Drawing;
using GraphicsPipeline;
namespace Utils;

public abstract class PresentableRenderTarget(int width, int height) : IRenderTarget
{
    protected readonly int[] BackBuffer = new int[width * height];
    protected readonly float[] DepthBuffer = new float[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;

    public abstract void Present();

    public void SetPixel(float x, float y, float z, in Color color)
    {
        var index = (int)(y * Width + x);
        var currentZ = DepthBuffer[index];

        while (z < currentZ)
        {
            if (ZTestInterloked(index, z, currentZ))
            {
                BackBuffer[index] = color.ToArgb();
                break;
            }
            currentZ = DepthBuffer[index];
        }
    }

    public bool ZTest(float x, float y, float z)
    {
        var index = (int)(y * Width + x);
        return DepthBuffer[index] > z;
    }


    public void Clear(in Color color)
    {
        Array.Fill(BackBuffer, color.ToArgb());
        Array.Fill(DepthBuffer, float.MaxValue);
    }

    private bool ZTestInterloked(int index, float z, float currentZ) =>
        Math.Abs(Interlocked.CompareExchange(ref DepthBuffer[index], z, currentZ) - currentZ) < float.Epsilon;
}
