using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Shaders;
using Utils;
using Utils.MeshLoader;
using Utils.TextureLoader;
namespace Benchmark;

public class ClippingBenchmark
{
    private readonly List<Triangle<Vertex>> _triangles = [];

    private int _trianglesCount1;
    private int _trianglesCount2;
    private int _trianglesCount3;
    private int _trianglesCount4;
    
    private readonly SimpleClipper<Vertex> _simpleClipper = new();
    private readonly Clipper<Vertex, VertexLinearInterpolator> _currentClipper = new();
    private readonly Clipper3<Vertex, VertexLinearInterpolator> _newClipper = new();
    private readonly Clipper4<Vertex, VertexLinearInterpolator> _newClipper2 = new();
    
    [Params(8, 5, 3)]
    public int CameraZ;
    
    [GlobalSetup]
    public async Task Setup()
    {
        var meshLoader = new ObjParser();
        var textureLoader = new PngTextureLoader();

        var model = new Model
        {
            Mesh = await meshLoader.LoadMeshAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\chest2.obj"),
            DiffuseMap = await textureLoader.LoadTextureAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\chest_diffuse2.png"),
            NormalMap = await textureLoader.LoadTextureAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\KittyChest_low_normal.png", true),
            SpecularMap = await textureLoader.LoadTextureAsync(@"C:\Users\coolp\OneDrive - bsuir.by\Рабочий стол\bsuir\3 курс\2 семестр\АКГ\ObjViewer\Benchmark\bin\Release\net8.0\ModelSamples\textures\chest_specular3.png"),
            Transform = new Transform
            {
                Position = new Vector3(0f, -2f, 0f),
                Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0),
                Scale = new Vector3(2f)
            }
        };

        var camera = new Camera(1920 / (float)1080);
        camera.Transform.Position = new Vector3(0, 2, CameraZ);
        camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);
        
        var vertexShader = new SimpleVertexShader
        {
            Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix,
            Model = model.Transform.WorldMatrix
        };

        for (var i = 0; i < model.Mesh.Indices.Count / 3; i++)
        {
            var triangleIndex = i * 3;
            Vertex a = model.Mesh.Vertices[model.Mesh.Indices[triangleIndex]];
            Vertex b = model.Mesh.Vertices[model.Mesh.Indices[triangleIndex + 1]];
            Vertex c = model.Mesh.Vertices[model.Mesh.Indices[triangleIndex + 2]];
            
            vertexShader.ProcessVertex(in a, out Vertex output1, out Vector4 clip1);
            vertexShader.ProcessVertex(in b, out Vertex output2, out Vector4 clip2);
            vertexShader.ProcessVertex(in c, out Vertex output3, out Vector4 clip3);
            
            if (CullTriangle(clip1, clip2, clip3))
                continue;
            
            var triangle = new Triangle<Vertex>
            {
                A = clip1,
                B = clip2,
                C = clip3,
                AData = output1,
                BData = output2,
                CData = output3
            };
            
            _triangles.Add(triangle);
        }
    }
    
    private static bool CullTriangle(in Vector4 position0, in Vector4 position1, in Vector4 position2)
    {
        Vector4 v0 = position1 - position0;
        Vector4 v1 = position2 - position0;
        Vector3 normal = Vector3.Cross(v0.AsVector128().AsVector3(), v1.AsVector128().AsVector3());
        return Vector3.Dot((-position0).AsVector128().AsVector3(), normal) >= 0;
    }
    
    [Benchmark]
    public void SimpleClipper()
    {
        var triangles = CollectionsMarshal.AsSpan(_triangles);
        for (var i = 0; i < triangles.Length; i++)
        {
            _trianglesCount1 += ClipTriangle(ref triangles[i], _simpleClipper);
        }
    }
    
    [Benchmark]
    public void CurrentClipper()
    {
        var triangles = CollectionsMarshal.AsSpan(_triangles);
        for (var i = 0; i < triangles.Length; i++)
        {
            _trianglesCount2 += ClipTriangle(ref triangles[i], _currentClipper);
        }
    }
    
    [Benchmark]
    public void NewClipper()
    {
        var triangles = CollectionsMarshal.AsSpan(_triangles);
        for (var i = 0; i < triangles.Length; i++)
        {
            _trianglesCount3 += ClipTriangle(ref triangles[i], _newClipper);
        }
    }
    
    [Benchmark]
    public void NewClipper2()
    {
        var triangles = CollectionsMarshal.AsSpan(_triangles);
        for (var i = 0; i < triangles.Length; i++)
        {
            _trianglesCount4 += ClipTriangle(ref triangles[i], _newClipper2);
        }
    }

    public void Run()
    {
        SimpleClipper();
        CurrentClipper();
        NewClipper();
        NewClipper2();

        Console.WriteLine(_trianglesCount1);
        Console.WriteLine(_trianglesCount2);
        Console.WriteLine(_trianglesCount3);
        Console.WriteLine(_trianglesCount4);
    }
    
    private static int ClipTriangle(ref Triangle<Vertex> triangle, IClipper<Vertex> clipper)
    {
        Span<Triangle<Vertex>> triangles = stackalloc Triangle<Vertex>[6];
        return clipper.ClipTriangle(in triangle, ref triangles);
    }
}
