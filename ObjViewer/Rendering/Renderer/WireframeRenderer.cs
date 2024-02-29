using GraphicsPipeline;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public sealed class WireframeRenderer : SimpleRenderer<SimpleFragmentShader, BresenhamRasterizer>
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        BackfaceCulling = false;
    }
}
