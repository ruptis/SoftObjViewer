using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class WireframeRenderer : SimpleRenderer<SimpleFragmentShader, BresenhamRasterizer>
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        BackfaceCulling = false;
    }
}
