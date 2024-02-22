using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class UvFragmentShader : IFragmentShader<Vertex>
{
    public Texture Texture { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        Vector3 pixel = Texture.SampleColor(input.TextureCoordinates);
        color = Color.FromArgb((byte)(pixel.X * 255), (byte)(pixel.Y * 255), (byte)(pixel.Z * 255));
    }
}
