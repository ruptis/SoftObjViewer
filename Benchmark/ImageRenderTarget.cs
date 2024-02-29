using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Color = System.Drawing.Color;
namespace Benchmark;

public sealed class ImageRenderTarget(Image<Rgba32> image) : PresentableRenderTarget(image.Width, image.Height)
{
    public override void Present()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            Color color = Color.FromArgb(BackBuffer[y * Width + x]);
            image[x, y] = new Rgba32(color.R, color.G, color.B, color.A);
        }
    }
}
