using System.Numerics;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Utils.Components;
using Utils.MeshLoader;
using Utils.TextureLoader;
namespace Benchmark.DepthTestBenchmarking;

public class DepthTestBenchmarks
{
    private readonly TexturedPhongRenderer _renderer1 = new();
    private readonly TexturedPhongRenderer2 _renderer2 = new();
    private readonly TexturedPhongRenderer2 _renderer3 = new();
    private readonly TexturedPhongRenderer2 _renderer4 = new();

    private readonly ImageRenderTarget _renderTarget1 = new(new Image<Rgba32>(1920, 1080));
    private readonly ImageRenderTarget2 _renderTarget2 = new(new Image<Rgba32>(1920, 1080));
    private readonly ImageRenderTarget3 _renderTarget3 = new(new Image<Rgba32>(1920, 1080));
    private readonly ImageRenderTarget4 _renderTarget4 = new(new Image<Rgba32>(1920, 1080));

    private readonly Camera _camera = new(1920 / 1080f);
    private Model _model;

    [GlobalSetup]
    public async Task Setup()
    {
        _camera.Transform.Position = new Vector3(5, 3, 7);
        _camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

        var meshLoader = new ObjParser();
        var textureLoader = new PngTextureLoader();

        _model = new Model
        {
            Mesh = await meshLoader.LoadMeshAsync(@"C:\Users\Pavel\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\SoftObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\chest2.obj"),
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
    }
    
    [Benchmark]
    public void EarlyDepthTest()
    {
        _renderer1.DrawModel(in _model, _camera, _renderTarget1);
    }
    
    [Benchmark]
    public void EarlyDepthTestWithMonitor()
    {
        _renderer2.DrawModel(in _model, _camera, _renderTarget2);
    }
    
    [Benchmark]
    public void EarlyDepthTestWithReadersWriterLockSlim()
    {
        _renderer3.DrawModel(in _model, _camera, _renderTarget3);
    }
    
    [Benchmark]
    public void EarlyDepthTestWithInterlocked()
    {
        _renderer4.DrawModel(in _model, _camera, _renderTarget4);
    }
}
