using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Shaders;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public class SimpleRenderer<TF, TR> : ModelRenderer<Vertex, SimpleVertexShader, TF, TR, Clipper<Vertex, VertexLinearInterpolator>>
    where TF : IFragmentShader<Vertex>, new()
    where TR : IRasterizer<Vertex>, new()
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;
    }
}
