using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using Utils;
namespace Benchmark.ClassBenckmarking;

public abstract class ModelRenderer<TFIn, TV, TF, TR, TC>
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
    where TC : IClipper<TFIn>, new()
{
    private readonly GraphicsPipeline<Vertex, TFIn> _graphicsPipeline2;

    protected ModelRenderer() =>
        _graphicsPipeline2 = new GraphicsPipeline<Vertex, TFIn>(VertexShader, FragmentShader, Rasterizer, Clipper);

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();
    protected TC Clipper { get; } = new();

    protected bool BackfaceCulling
    {
        get => _graphicsPipeline2.BackfaceCullingEnabled;
        set => _graphicsPipeline2.BackfaceCullingEnabled = value;
    }

    public void DrawModel(in Model model, Camera camera, IRenderTarget renderTarget)
    {
        OnDraw(model, camera, renderTarget);
        _graphicsPipeline2.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
    }

    protected abstract void OnDraw(in Model model, Camera camera, IRenderTarget renderTarget);
}

public class TexturedPhongRenderer : ModelRenderer<
    PhongShaderInput,
    PhongVertexShader,
    TexturedPhongFragmentShader,
    ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>,
    Clipper<PhongShaderInput, PhongLinearInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);

    protected override void OnDraw(in Model model, Camera camera, IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        VertexShader.ViewPosition = camera.Transform.Position;
        VertexShader.LightPosition = LightPosition;

        FragmentShader.NormalMap = model.NormalMap ?? Texture.Checkerboard512;
        FragmentShader.DiffuseMap = model.DiffuseMap ?? Texture.Checkerboard512;
        FragmentShader.SpecularMap = model.SpecularMap ?? Texture.Checkerboard512;
    }
}
