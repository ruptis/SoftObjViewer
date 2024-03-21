using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public class DepthRenderer : SimpleRenderer<ZFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>;
