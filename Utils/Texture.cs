using System.Numerics;
namespace Utils;

public readonly struct Texture(int width, int height, Vector3[] data)
{
    public static Texture Checkerboard256 { get; } = new(256, 256, CreateCheckerboard(256));
    public static Texture Checkerboard512 { get; } = new(512, 512, CreateCheckerboard(512));
    
    public ref Vector3 SampleColor(in Vector2 uv)
    {
        var index = GetSampleIndex(uv);
        return ref data[index];
    }
    
    public Vector3 SampleNormal(in Vector2 uv)
    {
        var index = GetSampleIndex(uv);
        return data[index] * 2.0f - Vector3.One;
    }

    private int GetSampleIndex(in Vector2 uv)
    {
        var x = (int)(uv.X * width) % width;
        var y = (int)(uv.Y * height) % height;
        return y * width + x;
    }

    private static Vector3[] CreateCheckerboard(int size)
    {
        var data = new Vector3[size * size];
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var index = (y * size + x);
                var color = (x / 16 + y / 16) % 2 == 0 ? 1.0f : 0.0f;
                data[index] = new Vector3(color);
            }
        }
        return data;
    }
}
