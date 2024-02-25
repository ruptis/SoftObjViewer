using GraphicsPipeline;
using GraphicsPipeline.Components;
using GraphicsPipeline.Components.Shaders;
namespace ObjViewer.Rendering.Renderer;

public class SimpleRenderer<TF, TR> : ModelRenderer<Vertex, SimpleVertexShader, TF, TR>
    where TF : IFragmentShader<Vertex>, new() 
    where TR : IRasterizer<Vertex>, new()
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;
    }
}