using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Benchmark;
using GraphicsPipeline.Components;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Rasterization.Interpolation;
using GraphicsPipeline.Components.Shaders;
using GraphicsPipeline.Components.Shaders.Phong;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
var meshLoader = new ObjParser();
var textureLoader = new PngTextureLoader();

var model = new Model
{
    Mesh = await meshLoader.LoadMeshAsync("ModelSamples/chest2.obj"),
    DiffuseMap = await textureLoader.LoadTextureAsync("ModelSamples/textures/chest_diffuse2.png"),
    NormalMap = await textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_normal.png", true),
    SpecularMap = await textureLoader.LoadTextureAsync("ModelSamples/textures/chest_specular3.png"),
    Transform = new Transform
    {
        Position = new Vector3(0f, -2f, 0f),
        Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0),
        Scale = new Vector3(2f)
    }
};

var image = new Image<Rgba32>(1920, 1080);

var renderTarget = new ImageRenderTarget(image);

var camera = new Camera(renderTarget.Width / (float)renderTarget.Height);
camera.Transform.Position = new Vector3(0, 2, 8);
camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

var vertexShader = new SimpleVertexShader();
var fragmentShader = new PhongFragmentShader();
var rasterizer = new HalfspaceTriangleRasterizer<Vertex,VertexHalfspaceInterpolator>();

var pipeline = new GraphicsPipeline.GraphicsPipeline<Vertex, Vertex>(
    vertexShader,
    fragmentShader,
    rasterizer);

vertexShader.Model = model.Transform.WorldMatrix;
vertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

fragmentShader.ViewPosition = camera.Transform.Position;
fragmentShader.LightPosition = new Vector3(0, 8, 8);

/*fragmentShader.NormalMap = model.NormalMap ?? Texture.Checkerboard512;
fragmentShader.DiffuseMap = model.DiffuseMap ?? Texture.Checkerboard512;
fragmentShader.SpecularMap = model.SpecularMap ?? Texture.Checkerboard512;*/
fragmentShader.Blinn = true;

var drawTimer = new Stopwatch();
var drawTimes = new List<TimeSpan>();

for (var frame = 0; frame < 200; frame++)
{
    drawTimer.Restart();
    renderTarget.Clear(System.Drawing.Color.SlateGray);
    pipeline.Render(model.Mesh.Vertices, model.Mesh.Indices, renderTarget);
    renderTarget.Present();
    drawTimer.Stop();
    drawTimes.Add(drawTimer.Elapsed);
}

image.Save("output.png");

var averageDrawTime = drawTimes.Skip(100).Average(t => t.TotalMilliseconds);

FileStream fileStream = File.OpenWrite("output.txt");
await using var writer = new StreamWriter(fileStream);
await writer.WriteLineAsync($"Average draw time: {averageDrawTime} ms");
await writer.WriteLineAsync($"Total draw time: {drawTimes.Sum(t => t.TotalMilliseconds)} ms");
await writer.WriteLineAsync($"Total frames: {drawTimes.Count}");
await writer.WriteLineAsync($"Average FPS: {1000.0 / averageDrawTime}");
await writer.WriteLineAsync($"Vertex count: {model.Mesh.Vertices.Count}");
await writer.WriteLineAsync($"Triangle count: {model.Mesh.Indices.Count / 3}");
await writer.WriteLineAsync($"Resolution: {image.Width}x{image.Height}");
await writer.WriteLineAsync("\n\n");
await writer.WriteLineAsync("Draw times:");
foreach (TimeSpan drawTime in drawTimes)
    await writer.WriteLineAsync(drawTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
await writer.FlushAsync();
Console.WriteLine("Benchmark finished");
