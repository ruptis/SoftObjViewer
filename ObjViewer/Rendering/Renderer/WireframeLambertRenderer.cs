﻿using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public sealed class WireframeLambertRenderer : SimpleRenderer<LambertFragmentShader, BresenhamRasterizer>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        BackfaceCulling = false;

        FragmentShader.LightPosition = LightPosition;
    }
}
