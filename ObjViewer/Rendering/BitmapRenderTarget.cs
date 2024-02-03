using System;
using System.Windows;
using System.Windows.Media.Imaging;
using GraphicsPipeline;
using Color = System.Drawing.Color;
namespace ObjViewer.Rendering;

public sealed class BitmapRenderTarget(WriteableBitmap bitmap) : IRenderTarget
{
    private readonly int[] _backBuffer = new int[bitmap.PixelWidth * bitmap.PixelHeight];
    private readonly Int32Rect _rect = new(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
    
    public int Width { get; } = bitmap.PixelWidth;
    public int Height { get; } = bitmap.PixelHeight;

    public void DrawPixel(int x, int y, in Color color)
    {
        _backBuffer[y * Width + x] = color.ToArgb();
    }
    
    public void Present() => bitmap.WritePixels(_rect, _backBuffer, Width * 4, 0);

    public void Clear(in Color color) => Array.Fill(_backBuffer, color.ToArgb());
}
