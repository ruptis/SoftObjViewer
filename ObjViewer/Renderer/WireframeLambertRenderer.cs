using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Simple;
using Utils.Components;
namespace ObjViewer.Renderer;

public sealed class WireframeLambertRenderer : SimpleRenderer<LambertFragmentShader, BresenhamRasterizer>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        BackfaceCulling = false;

        FragmentShader.LightPosition = LightPosition;
    }
}
