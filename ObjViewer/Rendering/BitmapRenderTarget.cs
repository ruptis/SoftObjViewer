using System.Windows;
using System.Windows.Media.Imaging;
using Utils;
namespace ObjViewer.Rendering;

public sealed class BitmapRenderTarget(WriteableBitmap bitmap) : PresentableRenderTarget(bitmap.PixelWidth, bitmap.PixelHeight)
{
    private readonly Int32Rect _rect = new(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);

    public override void Present() => bitmap.WritePixels(_rect, BackBuffer, Width * 4, 0);
}
