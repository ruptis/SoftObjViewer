using GraphicsPipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = System.Drawing.Color;
namespace Benchmark;

public sealed class ImageRenderTarget(Image<Rgba32> image) : IRenderTarget
{
    private readonly int[] _backBuffer = new int[image.Width * image.Height];
    private readonly float[] _depthBuffer = new float[image.Width * image.Height];

    public int Width { get; } = image.Width;
    public int Height { get; } = image.Height;

    public void DrawPixel(float x, float y, float z, in Color color)
    {
        var index = (int)(y * Width + x);
        if (_depthBuffer[index] <= z)
            return;

        _depthBuffer[index] = z;
        _backBuffer[index] = color.ToArgb();
    }

    public void Present()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            Color color = Color.FromArgb(_backBuffer[y * Width + x]);
            image[x, y] = new Rgba32(color.R, color.G, color.B, color.A);
        }
                
    }

    public void Clear(in Color color)
    {
        Array.Fill(_backBuffer, color.ToArgb());
        Array.Fill(_depthBuffer, float.MaxValue);
    }
}
