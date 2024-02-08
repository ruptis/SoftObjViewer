using System;
using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class LamberianFragmentShader : IFragmentShader<Vertex>
{
    public Vector3 LightPosition { get; set; }

    public void ProcessFragment(in Vector3 fragCoord, in Vertex input, out Color color)
    {
        Vector3 lightDirection = Vector3.Normalize(LightPosition - input.Position);
        var lightIntensity = Math.Max(Vector3.Dot(input.Normal, lightDirection), 0.0f);
        var colorComponent = (int)(lightIntensity * 255);
        colorComponent = Math.Min(colorComponent + 30, 255);
        color = Color.FromArgb(colorComponent, colorComponent, colorComponent);
    }
}
