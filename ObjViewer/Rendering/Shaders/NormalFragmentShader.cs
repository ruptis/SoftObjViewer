using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class NormalFragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector3 fragCoord, in Vertex input, out Color color)
    {
        Vector3 normal = Vector3.Normalize(input.Normal);
        color = Color.FromArgb(
            (int) (normal.X * 127 + 128),
            (int) (normal.Y * 127 + 128),
            (int) (normal.Z * 127 + 128)
        );
    }
}
