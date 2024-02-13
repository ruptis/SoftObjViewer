using System;
using System.Windows;
using System.Windows.Media.Imaging;
using GraphicsPipeline;
using Color = System.Drawing.Color;
namespace ObjViewer.Rendering;

public sealed class BitmapRenderTarget(WriteableBitmap bitmap) : IRenderTarget
{
    private readonly int[] _backBuffer = new int[bitmap.PixelWidth * bitmap.PixelHeight];
    private readonly float[] _depthBuffer = new float[bitmap.PixelWidth * bitmap.PixelHeight];
    private readonly Int32Rect _rect = new(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);

    public int Width { get; } = bitmap.PixelWidth;
    public int Height { get; } = bitmap.PixelHeight;

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
        bitmap.WritePixels(_rect, _backBuffer, Width * 4, 0);
    }

    public void Clear(in Color color)
    {
        Array.Fill(_backBuffer, color.ToArgb());
        Array.Fill(_depthBuffer, float.MaxValue);
    }
}
