using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using GraphicsPipeline.Components.Shaders.PostProcess;
using Utils;
namespace Benchmark.HdrBenchmarking;

public sealed class HdrPhongRenderer2
{
    private readonly GraphicsPipeline<Vertex, PhongShaderInput> _graphicsPipeline;

    private readonly PhongVertexShader _vertexShader = new();
    private readonly TexturedPhongFragmentShader _fragmentShader = new();
    private readonly ToneMappingPostProcessor _toneMappingPostProcessor = new();

    public HdrPhongRenderer2()
    {
        _graphicsPipeline = new GraphicsPipeline<Vertex, PhongShaderInput>(
            _vertexShader,
            _fragmentShader,
            new ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>(),
            new Clipper<PhongShaderInput, PhongLinearInterpolator>(),
            _toneMappingPostProcessor)
        {
            BackfaceCullingEnabled = true,
            DepthTestEnabled = true
        };
    }

    public void DrawModel(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        _vertexShader.Model = model.Transform.WorldMatrix;
        _vertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        _vertexShader.ViewPosition = camera.Transform.Position;
        _vertexShader.LightPosition = new Vector3(0, 8, 8);

        _fragmentShader.DiffuseMap = model.DiffuseMap;
        _fragmentShader.NormalMap = model.NormalMap;
        _fragmentShader.SpecularMap = model.SpecularMap;
        
        renderTarget.Clear(ClearColor);
        _graphicsPipeline.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
    }
    
    public Vector4 ClearColor { get; set; } = ColorUtils.SlateGray;
}
