using System.Drawing;
using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class ZFragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        var colorComponent = (byte)(fragCoord.W * 255);
        color = Color.FromArgb(colorComponent, colorComponent, colorComponent);
    }
}
