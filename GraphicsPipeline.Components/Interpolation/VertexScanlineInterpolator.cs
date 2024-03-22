﻿using System.Numerics;
using Utils;
using Utils.Components;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class VertexScanlineInterpolator : IScanlineInterpolator<Vertex>
{
    public Vertex Average(in Vertex v0, in Vertex v1, in Vertex v2) => new()
    {
        Position = (v0.Position + v1.Position + v2.Position) / 3,
        Normal = (v0.Normal + v1.Normal + v2.Normal) / 3,
        TextureCoordinates = (v0.TextureCoordinates + v1.TextureCoordinates + v2.TextureCoordinates) / 3
    };

    public Vertex Interpolate(in Vertex v0, in Vertex v1, float w0, float w1, float w, float amount) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, amount),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, amount),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates * w0, v1.TextureCoordinates * w1, amount) / w
    };
}
