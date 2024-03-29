﻿using System.Numerics;
using GraphicsPipeline.Components.Shaders.MultiLightPhong;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class MultiLightPhongScanlineInterpolator : IScanlineInterpolator<MultiLightPhongShaderInput>
{
    public MultiLightPhongShaderInput Average(in MultiLightPhongShaderInput v0, in MultiLightPhongShaderInput v1, in MultiLightPhongShaderInput v2) => new()
    {
        Position = (v0.Position + v1.Position + v2.Position) / 3,
        Normal = (v0.Normal + v1.Normal + v2.Normal) / 3,
        Tangent = (v0.Tangent + v1.Tangent + v2.Tangent) / 3,
        Bitangent = (v0.Bitangent + v1.Bitangent + v2.Bitangent) / 3,
        TextureCoordinates = (v0.TextureCoordinates + v1.TextureCoordinates + v2.TextureCoordinates) / 3
    };

    public MultiLightPhongShaderInput Interpolate(in MultiLightPhongShaderInput v0, in MultiLightPhongShaderInput v1, float w0, float w1, float w, float amount) => new()
    {
        Position = Vector3.Lerp(v0.Position, v1.Position, amount),
        Normal = Vector3.Lerp(v0.Normal, v1.Normal, amount),
        Tangent = Vector3.Lerp(v0.Tangent, v1.Tangent, amount),
        Bitangent = Vector3.Lerp(v0.Bitangent, v1.Bitangent, amount),
        TextureCoordinates = Vector2.Lerp(v0.TextureCoordinates * w0, v1.TextureCoordinates * w1, amount) / w,
    };
}