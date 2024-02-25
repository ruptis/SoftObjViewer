using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Rasterization.Interpolation;
using GraphicsPipeline.Components.Shaders;
namespace ObjViewer.Rendering.Renderer;

public class PhongRenderer : SimpleRenderer<PhongFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexScanlineInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        
        FragmentShader.ViewPosition = camera.Transform.Position;
        FragmentShader.LightPosition = LightPosition;
    }
}

public sealed class BlinnPhongRenderer : PhongRenderer
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        FragmentShader.Blinn = true;
    }
}