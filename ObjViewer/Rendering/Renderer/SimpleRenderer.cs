using GraphicsPipeline;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public class SimpleRenderer<TF, TR> : ModelRenderer<SimpleVertexShader, TF, TR>
    where TF : IFragmentShader<Vertex>, new() 
    where TR : IRasterizer<Vertex>, new()
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;
    }
}
