using System;
using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering;

public sealed class FragmentShader : IFragmentShader<Vertex>
{
    public void ProcessFragment(in Vector2 fragCoord, in Vertex input, out Color color)
    {
        var worldPos = input.Position;
        
        // make rainbow
        const float f = 0.5f;
        color = Color.FromArgb(
            (int)(MathF.Abs(MathF.Sin(worldPos.X * f)) * 255),
            (int)(MathF.Abs(MathF.Sin(worldPos.Y * f)) * 255),
            (int)(MathF.Abs(MathF.Sin(worldPos.Z * f)) * 255)
        );
    }
}
