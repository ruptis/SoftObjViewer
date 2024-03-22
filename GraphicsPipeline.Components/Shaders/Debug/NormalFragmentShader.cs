using System.Drawing;
using System.Numerics;
using Utils;
using Utils.Components;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class NormalFragmentShader : IFragmentShader<Vertex>
{
    public Texture? NormalTexture { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        Vector3 normal = NormalTexture?.SampleNormal(input.TextureCoordinates) ?? input.Normal;

        color = new Vector4(normal, 1.0f);
    }
}
