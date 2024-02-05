using GraphicsPipeline;
namespace ObjViewer.Rendering;

public sealed class WireframeRenderer : ModelRenderer<VertexShader, FragmentShader, BresenhamRasterizer>
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Projection = camera.ProjectionMatrix;
        VertexShader.View = camera.ViewMatrix;
    }
}
