using System.Drawing;
using System.Windows.Threading;
using GraphicsPipeline;
using GraphicsPipeline.Components;
namespace ObjViewer.Rendering.Renderer;

public abstract class ModelRenderer<TFIn, TV, TF, TR> : IModelRenderer
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
{
    private readonly GraphicsPipeline<Vertex, TFIn> _graphicsPipeline;

    protected ModelRenderer() => 
        _graphicsPipeline = new GraphicsPipeline<Vertex, TFIn>(VertexShader, FragmentShader, Rasterizer);

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();

    protected bool BackfaceCulling
    {
        get => _graphicsPipeline.BackfaceCullingEnabled; 
        set => _graphicsPipeline.BackfaceCullingEnabled = value;
    }

    public Color ClearColor { get; set; } = Color.SlateGray;

    public void DrawModel(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        OnDraw(model, camera, renderTarget);
        renderTarget.Clear(ClearColor);
        _graphicsPipeline.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
    }

    protected abstract void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget);
}
