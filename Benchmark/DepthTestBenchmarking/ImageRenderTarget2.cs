using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace Benchmark.DepthTestBenchmarking;

public sealed class ImageRenderTarget2(Image<Rgba32> image) : PresentableRenderTarget2<byte>(image.Width, image.Height)
{
    public override void Present()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            var color = BackBuffer[y * Width + x];
            image[x, y] = new Rgba32(color.R, color.G, color.B, color.A);
        }
    }
    protected override Pixel TransformColor(in Vector4 color)
    {
        Vector4 clampedColor = Vector4.Clamp(color, Vector4.Zero, Vector4.One);
        clampedColor *= new Vector4(byte.MaxValue);
        return new Pixel
        {
            R = (byte)clampedColor.X,
            G = (byte)clampedColor.Y,
            B = (byte)clampedColor.Z,
            A = (byte)clampedColor.W
        };
    }
}
