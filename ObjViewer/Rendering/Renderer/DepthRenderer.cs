using GraphicsPipeline.Components;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Rasterization.Interpolation;
using GraphicsPipeline.Components.Shaders.Debug;
namespace ObjViewer.Rendering.Renderer;

public class DepthRenderer : SimpleRenderer<ZFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>;