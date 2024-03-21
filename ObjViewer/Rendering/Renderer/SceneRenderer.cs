using System.Numerics;
using GraphicsPipeline;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public abstract class SceneRenderer<TFIn, TV, TF, TR, TC> : ISceneRenderer
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
    where TC : IClipper<TFIn>, new()
{
    private readonly GraphicsPipeline<Vertex, TFIn> _graphicsPipeline;

    protected SceneRenderer(IPostProcessor? postProcessor = null) =>
        _graphicsPipeline = new GraphicsPipeline<Vertex, TFIn>(VertexShader, FragmentShader, Rasterizer, Clipper, postProcessor);

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();
    protected TC Clipper { get; } = new();

    protected bool BackfaceCulling
    {
        get => _graphicsPipeline.BackfaceCullingEnabled;
        set => _graphicsPipeline.BackfaceCullingEnabled = value;
    }
    
    public Vector4 ClearColor { get; set; } = ColorUtils.SlateGray;

    public void RenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        OnRenderScene(in scene, renderTarget);
        renderTarget.Clear(ClearColor);
        _graphicsPipeline.Render(scene.Model.Mesh.Vertices, scene.Model.Mesh.Indices, renderTarget);
    }

    protected abstract void OnRenderScene(in Scene scene, IRenderTarget renderTarget);
}
