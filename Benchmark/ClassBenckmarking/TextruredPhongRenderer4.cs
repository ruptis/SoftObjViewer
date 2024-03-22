using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using Utils;
using Utils.Components;
namespace Benchmark.ClassBenckmarking;

public abstract class ModelRenderer4<TFIn, TV, TF, TR, TC>
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
    where TC : IClipper<TFIn>, new()
{
    private readonly GraphicsPipeline<Vertex, TFIn> _graphicsPipeline;

    protected ModelRenderer4() =>
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

    public void DrawModel(in ModelWithTextureClass model, Camera camera, IRenderTarget renderTarget)
    {
        OnDraw(model, camera, renderTarget);
        _graphicsPipeline.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
    }

    protected abstract void OnDraw(in ModelWithTextureClass model, Camera camera, IRenderTarget renderTarget);
}

public class TexturedPhongRenderer4 : ModelRenderer4<
    PhongShaderInput,
    PhongVertexShader,
    TexturedPhongFragmentShader2,
    ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>,
    Clipper<PhongShaderInput, PhongLinearInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);

    protected override void OnDraw(in ModelWithTextureClass model, Camera camera, IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        VertexShader.ViewPosition = camera.Transform.Position;
        VertexShader.LightPosition = LightPosition;

        FragmentShader.NormalMap = model.NormalMap ?? TextureClass.Empty;
        FragmentShader.DiffuseMap = model.DiffuseMap ?? TextureClass.Empty;
        FragmentShader.SpecularMap = model.SpecularMap ?? TextureClass.Empty;
    }
}
