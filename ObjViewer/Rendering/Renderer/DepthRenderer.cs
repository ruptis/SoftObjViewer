using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils;
using Utils.Components;
namespace ObjViewer.Rendering.Renderer;

public class DepthRenderer : SimpleRenderer<ZFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>;
