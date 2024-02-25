using GraphicsPipeline;
using GraphicsPipeline.Components;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Rasterization.Interpolation;
using GraphicsPipeline.Components.Shaders.Debug;
namespace ObjViewer.Rendering.Renderer;

public sealed class NormalRenderer : SimpleRenderer<NormalFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(in model, in camera, renderTarget);
        FragmentShader.NormalTexture = model.NormalMap;
    }
}
