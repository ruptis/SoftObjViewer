using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utils.RenderTargets;
namespace ObjViewer;

public sealed class BitmapRenderTarget(
    int width,
    int height) : PresentableRenderTarget<uint>(width, height)
{
    private readonly Int32Rect _rect = new(0, 0, width, height);
    private readonly WriteableBitmap _bitmap = new(width, height, 96, 96, PixelFormats.Bgra32, null);

    public BitmapSource Bitmap => _bitmap;

    public override void Present() => _bitmap.WritePixels(_rect, ColorBuffer, Width * 4, 0);
    
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
