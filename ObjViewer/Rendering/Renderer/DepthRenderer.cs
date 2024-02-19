using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public class DepthRenderer : SimpleRenderer<ZFragmentShader, ScanlineTriangleRasterizer>;