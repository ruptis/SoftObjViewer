using System.Numerics;
namespace ObjViewer.Rendering;

public readonly struct Texture(int width, int height, byte[] data)
{
    public static Texture Checkerboard256 { get; } = new(256, 256, CreateCheckerboard(256));
    public static Texture Checkerboard512 { get; } = new(512, 512, CreateCheckerboard(512));
    
    private int Width { get; } = width;
    private int Height { get; } = height;
    private byte[] Data { get; } = data;

    public Vector3 SampleColor(in Vector2 uv)
    {
        var x = (int)(uv.X * Width) % Width;
        var y = (int)(uv.Y * Height) % Height;
        var index = (y * Width + x) * 3;
        return new Vector3(Data[index], Data[index + 1], Data[index + 2]) / 255.0f;
    }
    
    public Vector3 SampleNormal(in Vector2 uv)
    {
        var x = (int)(uv.X * Width) % Width;
        var y = (int)(uv.Y * Height) % Height;
        var index = (y * Width + x) * 3;
        return new Vector3(Data[index], 255 - Data[index + 1], Data[index + 2]) / 127.5f - Vector3.One;
    }

    private static byte[] CreateCheckerboard(int size)
    {
        var data = new byte[size * size * 3];
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var index = (y * size + x) * 3;
                var color = (x / 16 + y / 16) % 2 == 0 ? 255 : 0;
                data[index] = (byte)color;
                data[index + 1] = (byte)color;
                data[index + 2] = (byte)color;
            }
        }
        return data;
    }
}
