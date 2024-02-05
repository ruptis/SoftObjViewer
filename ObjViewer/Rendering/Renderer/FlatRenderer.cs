using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class FlatRenderer : ModelRenderer<SimpleVertexShader, LamberianFragmentShader, TriangleRasterizer>
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;
    }
}
