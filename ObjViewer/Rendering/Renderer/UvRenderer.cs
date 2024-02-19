using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public sealed class UvRenderer : SimpleRenderer<UvFragmentShader, ScanlineTriangleRasterizer>;