using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Simple;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public class PhongRenderer : SimpleRenderer<PhongFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        FragmentShader.ViewPosition = scene.Camera.Transform.Position;
        FragmentShader.LightPosition = LightPosition;
    }
}
public sealed class BlinnPhongRenderer : PhongRenderer
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        FragmentShader.Blinn = true;
    }
}
