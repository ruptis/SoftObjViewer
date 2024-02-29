using System.Numerics;
using Benchmark;
using BenchmarkDotNet.Running;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Color = System.Drawing.Color;

BenchmarkRunner.Run<Benchmarks>();

return;

var image = new Image<Rgba32>(1920, 1080);
var rt = new ImageRenderTarget(image);

var vertices = new Vertex[]
{
    new()
    {
        Position = new Vector3(-1, -1, 0)
    },
    new()
    {
        Position = new Vector3(1, -1, 0)
    },
    new()
    {
        Position = new Vector3(0, 2, 0)
    }
};

var triangleTransform = new Transform();

var camera = new Camera(1920 / (float)1080);
camera.Transform.Position = new Vector3(0, 0, 2);
camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

var vertexShader = new SimpleVertexShader
{
    Mvp = triangleTransform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix,
    Model = triangleTransform.WorldMatrix
};

var fragmentShader = new SimpleFragmentShader();

var rasterizer = new BresenhamRasterizer();

var clipper = new Clipper<Vertex, VertexLinearInterpolator>();

var viewportMatrix = Matrix4x4.CreateViewportLeftHanded(0, 0, 1920 - 1, 1080 - 1, 0, 1);
rt.Clear(Color.SlateGray);

vertexShader.ProcessVertex(in vertices[0], out Vertex output1, out Vector4 clip1);
vertexShader.ProcessVertex(in vertices[1], out Vertex output2, out Vector4 clip2);
vertexShader.ProcessVertex(in vertices[2], out Vertex output3, out Vector4 clip3);

var triangle = new Triangle<Vertex>
{
    A = clip1,
    B = clip2,
    C = clip3,
    AData = output1,
    BData = output2,
    CData = output3
};

Span<Triangle<Vertex>> clippedTriangles = stackalloc Triangle<Vertex>[10];
var count = clipper.ClipTriangle(in triangle, ref clippedTriangles);

for (var i = 0; i < count; i++)
{
    ToScreenSpace(ref clippedTriangles[i]);

    rasterizer.Rasterize(in clippedTriangles[i], fragment =>
    {
        fragmentShader.ProcessFragment(in fragment.Position, in fragment.Data, out Color color);
        rt.SetPixel(fragment.Position.X, fragment.Position.Y, fragment.Position.Z, in color);
    });
}

rt.Present();
image.Save("output.png");

void ToScreenSpace(ref Triangle<Vertex> triangle)
{
    var wA = ToNdc(ref triangle.A);
    var wB = ToNdc(ref triangle.B);
    var wC = ToNdc(ref triangle.C);
    triangle.A = Vector4.Transform(triangle.A, viewportMatrix);
    triangle.B = Vector4.Transform(triangle.B, viewportMatrix);
    triangle.C = Vector4.Transform(triangle.C, viewportMatrix);

    triangle.A.W = wA;
    triangle.B.W = wB;
    triangle.C.W = wC;
}

float ToNdc(ref Vector4 position)
{
    var w = 1.0f / position.W;
    position *= w;
    return w;
}
