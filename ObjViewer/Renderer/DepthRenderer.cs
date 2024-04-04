using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils.Components;
namespace ObjViewer.Renderer;

public class DepthRenderer : SimpleRenderer<DepthFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>;
