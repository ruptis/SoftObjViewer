using System.Drawing;
using System.Numerics;
using Utils;
using Utils.Components;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class ZFragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        color = new Vector4(input.Position.Z, input.Position.Z, input.Position.Z, 1.0f);
    }
}
