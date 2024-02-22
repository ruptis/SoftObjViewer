using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Rasterization.Interpolation;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public class DepthRenderer : SimpleRenderer<ZFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexInterpolator>>;