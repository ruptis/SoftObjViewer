using System.Drawing;
using System.Numerics;
using Utils;
using Utils.Components;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class UvFragmentShader : IFragmentShader<Vertex>
{
    public Texture Texture { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        Vector3 pixel = Texture.SampleColor(input.TextureCoordinates);
        color = new Vector4(pixel, 1.0f);
    }
}
