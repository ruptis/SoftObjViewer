using System.Drawing;
using GraphicsPipeline;

namespace ObjViewer.Rendering;

public abstract class ModelRenderer<TV, TF, TR> : IModelRenderer 
    where TV : IVertexShader<Vertex, Vertex>, new()
    where TF : IFragmentShader<Vertex>, new()
    where TR : IRasterizer<Vertex>, new()
{
    private readonly GraphicsPipeline<Vertex, Vertex> _graphicsPipeline;

    protected ModelRenderer() => 
        _graphicsPipeline = new GraphicsPipeline<Vertex, Vertex>(VertexShader, FragmentShader, Rasterizer);

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();

    public Color ClearColor { get; set; } = Color.SlateGray;

    public void DrawModel(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        OnDraw(model, camera, renderTarget);
        renderTarget.Clear(ClearColor);
        _graphicsPipeline.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
        renderTarget.Present();
    }

    protected abstract void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget);
}
