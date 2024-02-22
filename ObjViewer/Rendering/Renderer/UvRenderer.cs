using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Rasterization.Interpolation;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class UvRenderer : SimpleRenderer<UvFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexInterpolator>>
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        base.OnDraw(in model, in camera, in renderTarget);

        FragmentShader.Texture = model.DiffuseMap ?? Texture.Checkerboard512;
    }
}