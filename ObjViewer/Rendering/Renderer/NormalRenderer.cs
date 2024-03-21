using GraphicsPipeline;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public sealed class NormalRenderer : SimpleRenderer<NormalFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        FragmentShader.NormalTexture = scene.Model.NormalTexture;
    }
}
