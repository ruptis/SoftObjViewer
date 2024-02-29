using GraphicsPipeline;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public sealed class UvRenderer : SimpleRenderer<UvFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(in model, in camera, renderTarget);

        FragmentShader.Texture = model.DiffuseMap ?? Texture.Checkerboard512;
    }
}
