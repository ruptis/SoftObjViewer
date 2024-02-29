﻿using System.Drawing;
using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders;

public sealed class LambertFragmentShader : IFragmentShader<Vertex>
{
    public Vector3 LightPosition { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        Vector3 lightDirection = Vector3.Normalize(LightPosition - input.Position);
        var lightIntensity = Math.Max(Vector3.Dot(input.Normal, lightDirection), 0.0f);
        var colorComponent = (byte)(lightIntensity * 255);
        color = Color.FromArgb(colorComponent, colorComponent, colorComponent);
    }
}