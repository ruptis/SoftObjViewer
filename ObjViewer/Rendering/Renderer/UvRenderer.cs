using GraphicsPipeline;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils;
using Utils.Components;
namespace ObjViewer.Rendering.Renderer;

public sealed class UvRenderer : SimpleRenderer<UvFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        FragmentShader.Texture = scene.Model.DiffuseMap;
    }
}
