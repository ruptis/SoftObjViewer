using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class UvFragmentShader : IFragmentShader<Vertex>
{
    private const int Width = 256;
    private const int Height = 256;
    private readonly Color[] _texture = CreateTestTexture();
    
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        var x = (int)(input.TextureCoordinates.X * Width) % Width;
        var y = (int)(input.TextureCoordinates.Y * Height) % Height;
        color = _texture[y * Width + x];
    }
    
    private static Color[] CreateTestTexture()
    {
        var texture = new Color[Width * Height];
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Color color = (x / 8 + y / 8) % 2 == 0 ? Color.DarkGray : Color.Black;
                texture[y * Width + x] = color;
            }
        }
        return texture;
    }
}
