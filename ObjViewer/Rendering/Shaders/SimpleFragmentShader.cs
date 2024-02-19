using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public class SimpleFragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        color = Color.Orange;
    }
}
