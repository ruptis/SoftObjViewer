using System.Numerics;
using Utils.Components;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class DepthFragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        color = new Vector4(fragCoord.W, fragCoord.W, fragCoord.W, 1.0f);
    }
}
