﻿using System.Numerics;
using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class WireframeLambertRenderer : ModelRenderer<SimpleVertexShader, LamberianFragmentShader, BresenhamRasterizer>
{
    private static readonly Vector3 LightPosition = new(0, 2, 8);
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;
        
        FragmentShader.LightPosition = LightPosition;
    }
}