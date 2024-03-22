using System.Drawing;
using System.Numerics;
using Utils;
using Utils.Components;
using Utils.Utils;
namespace GraphicsPipeline.Components.Shaders.Simple;

public class SimpleFragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        color = Color.Orange.ToVector();
    }
}
