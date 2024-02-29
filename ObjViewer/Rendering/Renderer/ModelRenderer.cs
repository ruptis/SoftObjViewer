using GraphicsPipeline;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public abstract class ModelRenderer<TFIn, TV, TF, TR, TC> : IModelRenderer
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
    where TC : IClipper<TFIn>, new()
{
    private readonly GraphicsPipeline<Vertex, TFIn> _graphicsPipeline;

    protected ModelRenderer() =>
        _graphicsPipeline = new GraphicsPipeline<Vertex, TFIn>(VertexShader, FragmentShader, Rasterizer, Clipper);

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();
    protected TC Clipper { get; } = new();

    protected bool BackfaceCulling
    {
        get => _graphicsPipeline.BackfaceCullingEnabled;
        set => _graphicsPipeline.BackfaceCullingEnabled = value;
    }

    public void DrawModel(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        OnDraw(model, camera, renderTarget);
        _graphicsPipeline.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
    }

    protected abstract void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget);
}
