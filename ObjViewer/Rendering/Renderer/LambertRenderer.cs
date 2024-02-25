﻿using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Rasterization.Interpolation;
using GraphicsPipeline.Components.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class LambertRenderer : SimpleRenderer<LambertFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);

        FragmentShader.LightPosition = LightPosition;
    }
}
