using System;
using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering;

public sealed class FragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector3 fragCoord, in Vertex input, out Color color)
    {
        // simple lambertian shading
        var lightDir = Vector3.Normalize(new Vector3(1.0f, 1.0f, 1.0f));
        var diffuse = Math.Max(Vector3.Dot(input.Normal, lightDir), 0.0f);
        color = Color.FromArgb((int)(diffuse * 255), (int)(diffuse * 255), (int)(diffuse * 255));
        // make brighter
        color = Color.FromArgb(Math.Min(color.R + 50, 255), Math.Min(color.G + 50, 255), Math.Min(color.B + 50, 255));
    }
}
