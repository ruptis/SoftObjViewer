﻿using GraphicsPipeline;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Simple;
using Utils.Components;
namespace ObjViewer.Renderer;

public sealed class WireframeRenderer : SimpleRenderer<SimpleFragmentShader, BresenhamRasterizer>
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        BackfaceCulling = false;
    }
}
