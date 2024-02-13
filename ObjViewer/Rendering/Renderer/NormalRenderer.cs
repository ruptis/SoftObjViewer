using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class NormalRenderer : SimpleRenderer<NormalFragmentShader, TriangleRasterizer>;
