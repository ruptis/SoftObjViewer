using System.Numerics;
using BenchmarkDotNet.Attributes;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Utils.Components;
using Utils.MeshLoader;
using Utils.TextureLoader;
using Utils.Utils;
using Color = System.Drawing.Color;
namespace Benchmark.ClipperBenchmarking;

public class ClipperBenchmarks
{
    private const int MaxWidth = 1920;
    private const int MaxHeight = 1080;

    private Model _model;
    private readonly ImageRenderTarget[] _renderTargets = new ImageRenderTarget[4];
    private readonly GraphicsPipeline<Vertex, PhongShaderInput>[] _pipelines = new GraphicsPipeline<Vertex, PhongShaderInput>[4];

    [Params(14, 8, 5, 3)]
    public int CameraZ;

    [GlobalSetup]
    public async Task Setup()
    {
        for (var i = 0; i < _renderTargets.Length; i++)
            _renderTargets[i] = new ImageRenderTarget(new Image<Rgba32>(MaxWidth, MaxHeight));

        var meshLoader = new ObjParser();
        var textureLoader = new PngTextureLoader();

        _model = new Model
        {
            Mesh = await meshLoader.LoadMeshAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\chest2.obj"),
            Transform = new Transform
            {
                Position = new Vector3(0f, -2f, 0f),
                Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0),
                Scale = new Vector3(2f)
            },
            DiffuseMap = await textureLoader.LoadTextureAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\chest_diffuse2.png"),
            NormalMap = await textureLoader.LoadTextureAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\KittyChest_low_normal.png", true),
            SpecularMap = await textureLoader.LoadTextureAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\chest_specular3.png"),
        };

        var camera = new Camera(MaxWidth / (float)MaxHeight);
        camera.Transform.Position = new Vector3(0, 2, CameraZ);
        camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

        _pipelines[0] = SetupScanline(camera, new SimpleClipper<PhongShaderInput>());
        _pipelines[0].ScissorTestEnabled = true;
        _pipelines[1] = SetupHalfspace(camera, new SimpleClipper<PhongShaderInput>());
        _pipelines[1].ScissorTestEnabled = true;
        _pipelines[2] = SetupScanline(camera, new Clipper<PhongShaderInput, PhongLinearInterpolator>());
        _pipelines[3] = SetupHalfspace(camera, new Clipper<PhongShaderInput, PhongLinearInterpolator>());

        foreach (ImageRenderTarget rt in _renderTargets)
            rt.Clear(Color.SlateGray.ToVector());
    }

    private GraphicsPipeline<Vertex, PhongShaderInput> SetupScanline(Camera camera, IClipper<PhongShaderInput> clipper)
    {
        var vertexShader = new PhongVertexShader();
        var fragmentShader = new TexturedPhongFragmentShader();
        var rasterizer = new ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>();

        vertexShader.Model = _model.Transform.WorldMatrix;
        vertexShader.Mvp = _model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        vertexShader.ViewPosition = camera.Transform.Position;
        vertexShader.LightPosition = new Vector3(0, 8, 8);

        fragmentShader.DiffuseMap = _model.DiffuseMap;
        fragmentShader.NormalMap = _model.NormalMap;
        fragmentShader.SpecularMap = _model.SpecularMap;

        fragmentShader.Blinn = true;

        return new GraphicsPipeline<Vertex, PhongShaderInput>(
            vertexShader,
            fragmentShader,
            rasterizer,
            clipper);
    }

    private GraphicsPipeline<Vertex, PhongShaderInput> SetupHalfspace(Camera camera, IClipper<PhongShaderInput> clipper)
    {
        var vertexShader = new PhongVertexShader();
        var fragmentShader = new TexturedPhongFragmentShader();
        var rasterizer = new HalfspaceTriangleRasterizer<PhongShaderInput, PhongHalfspaceInterpolator>();

        vertexShader.Model = _model.Transform.WorldMatrix;
        vertexShader.Mvp = _model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        vertexShader.ViewPosition = camera.Transform.Position;
        vertexShader.LightPosition = new Vector3(0, 8, 8);

        fragmentShader.DiffuseMap = _model.DiffuseMap;
        fragmentShader.NormalMap = _model.NormalMap;
        fragmentShader.SpecularMap = _model.SpecularMap;

        fragmentShader.Blinn = true;

        return new GraphicsPipeline<Vertex, PhongShaderInput>(
            vertexShader,
            fragmentShader,
            rasterizer,
            clipper);
    }

    [Benchmark]
    public void ScanlineWithSimpleClipper()
    {
        _pipelines[0].Render(_model.Mesh.Vertices, _model.Mesh.Indices, _renderTargets[2]);
    }

    [Benchmark]
    public void HalfspaceWithSimpleClipper()
    {
        _pipelines[1].Render(_model.Mesh.Vertices, _model.Mesh.Indices, _renderTargets[3]);
    }

    [Benchmark]
    public void ScanlineWithAdvancedClipper()
    {
        _pipelines[2].Render(_model.Mesh.Vertices, _model.Mesh.Indices, _renderTargets[2]);
    }

    [Benchmark]
    public void HalfspaceWithAdvancedClipper()
    {
        _pipelines[3].Render(_model.Mesh.Vertices, _model.Mesh.Indices, _renderTargets[3]);
    }
}
