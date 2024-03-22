using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Utils.RenderTargets;
namespace Benchmark;

public sealed class ImageRenderTarget(Image<Rgba32> image) : PresentableRenderTarget<uint>(image.Width, image.Height)
{
    public override void Present()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            var color = ColorBuffer[y * Width + x];
            image[x, y] = new Rgba32(color);
        }
    }
    
    protected override uint TransformColor(in Vector4 color)
    {
        Vector4 clampedColor = Vector4.Clamp(color, Vector4.Zero, Vector4.One);
        clampedColor *= byte.MaxValue;
        var r = (byte)clampedColor.X;
        var g = (byte)clampedColor.Y;
        var b = (byte)clampedColor.Z;
        var a = (byte)clampedColor.W;
        
        return (uint)(a << 24 | r << 16 | g << 8 | b);
    }
}
