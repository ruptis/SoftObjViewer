using System.Drawing;
using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class UvFragmentShader : IFragmentShader<Vertex>
{
    public Texture Texture { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        Vector3 pixel = Texture.SampleColor(input.TextureCoordinates);
        color = Color.FromArgb((byte)(pixel.X * 255), (byte)(pixel.Y * 255), (byte)(pixel.Z * 255));
    }
}
