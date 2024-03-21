using System.Numerics;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Utils.MeshLoader;
using Utils.TextureLoader;
namespace Benchmark.ClassBenckmarking;

public class ClassBenchmarks
{
    private Model _model1;
    private ModelClassWithMeshClassAndTextureClass _model2 = null!;
    private ModelClassWithMeshClass _model3 = null!;
    private ModelWithTextureClass _model4;

    private readonly TexturedPhongRenderer _renderer1 = new();
    private readonly TexturedPhongRenderer2 _renderer2 = new();
    private readonly TexturedPhongRenderer3 _renderer3 = new();
    private readonly TexturedPhongRenderer4 _renderer4 = new();
    
    private readonly Camera _camera = new(1920/1080f);
    private readonly ImageRenderTarget _renderTarget1 = new(new Image<Rgba32>(1920, 1080));
    private readonly ImageRenderTarget _renderTarget2 = new(new Image<Rgba32>(1920, 1080));
    private readonly ImageRenderTarget _renderTarget3 = new(new Image<Rgba32>(1920, 1080));
    private readonly ImageRenderTarget _renderTarget4 = new(new Image<Rgba32>(1920, 1080));
    

    [GlobalSetup]
    public async Task Setup()
    {
        
        
        var meshLoader = new ObjParser();
        var textureLoader = new PngTextureLoader();
        Mesh mesh = await meshLoader.LoadMeshAsync(@"C:\Users\Pavel\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\SoftObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\chest2.obj");
        Texture diffuseMap = await textureLoader.LoadTextureAsync(@"C:\Users\Pavel\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\SoftObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\chest_diffuse2.png");
        Texture normalMap = await textureLoader.LoadTextureAsync(@"C:\Users\Pavel\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\SoftObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\KittyChest_low_normal.png", true);
        Texture specularMap = await textureLoader.LoadTextureAsync(@"C:\Users\Pavel\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\SoftObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\chest_specular3.png");

        var transform = new Transform
        {
            Position = new Vector3(0f, -2f, 0f),
            Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0),
            Scale = new Vector3(2f)
        };

        _model1 = new Model
        {
            Mesh = mesh,
            Transform = transform,
            DiffuseMap = diffuseMap,
            NormalMap = normalMap,
            SpecularMap = specularMap
        };

        _model2 = new ModelClassWithMeshClassAndTextureClass(
            new MeshClass(new List<Vertex>(mesh.Vertices), new List<int>(mesh.Indices)),
            transform,
            new TextureClass(diffuseMap.Width, diffuseMap.Height, diffuseMap.Data.ToArray()),
            new TextureClass(normalMap.Width, normalMap.Height, normalMap.Data.ToArray()),
            new TextureClass(specularMap.Width, specularMap.Height, specularMap.Data.ToArray()));

        _model3 = new ModelClassWithMeshClass(
            new MeshClass(mesh.Vertices, mesh.Indices),
            transform,
            diffuseMap,
            normalMap,
            specularMap);

        _model4 = new ModelWithTextureClass
        {
            Mesh = mesh,
            Transform = transform,
            DiffuseMap = new TextureClass(diffuseMap.Width, diffuseMap.Height, diffuseMap.Data),
            NormalMap = new TextureClass(normalMap.Width, normalMap.Height, normalMap.Data),
            SpecularMap = new TextureClass(specularMap.Width, specularMap.Height, specularMap.Data)
        };
    }
    
    [Benchmark]
    public void StructModel()
    {
        _renderer1.DrawModel(in _model1, _camera, _renderTarget1); 
    }
    
    [Benchmark]
    public void ClassModelWithClassMeshAndClassTextures()
    {
        _renderer2.DrawModel(_model2, _camera, _renderTarget2); 
    }
    
    [Benchmark]
    public void ClassModelWithClassMesh()
    {
        _renderer3.DrawModel(_model3, _camera, _renderTarget3); 
    }
    
    [Benchmark]
    public void StructModelWithClassTextures()
    {
        _renderer4.DrawModel(_model4, _camera, _renderTarget4); 
    }
}
