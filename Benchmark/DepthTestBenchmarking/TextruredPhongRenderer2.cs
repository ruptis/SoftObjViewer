using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using Utils;
namespace Benchmark.DepthTestBenchmarking;

public abstract class ModelRenderer2<TFIn, TV, TF, TR, TC>
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
    where TC : IClipper<TFIn>, new()
{
    private readonly GraphicsPipeline2<Vertex, TFIn> _graphicsPipeline2;

    protected ModelRenderer2() =>
        _graphicsPipeline2 = new GraphicsPipeline2<Vertex, TFIn>(VertexShader, FragmentShader, Rasterizer, Clipper);

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();
    protected TC Clipper { get; } = new();

    protected bool BackfaceCulling
    {
        get => _graphicsPipeline2.BackfaceCullingEnabled;
        set => _graphicsPipeline2.BackfaceCullingEnabled = value;
    }

    public void DrawModel(in Model model, in Camera camera, IRenderTarget2 renderTarget2)
    {
        OnDraw(model, camera, renderTarget2);
        _graphicsPipeline2.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget2);
    }

    protected abstract void OnDraw(in Model model, in Camera camera, IRenderTarget2 renderTarget2);
}

public class TexturedPhongRenderer2 : ModelRenderer2<
    PhongShaderInput,
    PhongVertexShader,
    TexturedPhongFragmentShader,
    ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>,
    Clipper<PhongShaderInput, PhongLinearInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);

    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget2 renderTarget2)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        VertexShader.ViewPosition = camera.Transform.Position;
        VertexShader.LightPosition = LightPosition;

        FragmentShader.NormalMap = model.NormalMap;
        FragmentShader.DiffuseMap = model.DiffuseMap;
        FragmentShader.SpecularMap = model.SpecularMap;
    }
}
