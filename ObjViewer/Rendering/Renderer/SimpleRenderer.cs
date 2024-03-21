using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Shaders.Simple;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public class SimpleRenderer<TF, TR> : SceneRenderer<Vertex, SimpleVertexShader, TF, TR, Clipper<Vertex, VertexLinearInterpolator>>
    where TF : IFragmentShader<Vertex>, new()
    where TR : IRasterizer<Vertex>, new()
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        VertexShader.Model = scene.Model.Transform.WorldMatrix;
        VertexShader.Mvp = scene.Model.Transform.WorldMatrix * scene.Camera.ViewMatrix * scene.Camera.ProjectionMatrix;
    }
}
