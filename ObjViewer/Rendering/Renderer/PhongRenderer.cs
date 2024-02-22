using System.Numerics;
using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Rasterization.Interpolation;
using ObjViewer.Rendering.Shaders;
namespace ObjViewer.Rendering.Renderer;

public class PhongRenderer : SimpleRenderer<PhongFragmentShader, ScanlineTriangleRasterizer<Vertex, VertexInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 2, 8);
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        
        FragmentShader.ViewPosition = camera.Transform.Position;
        FragmentShader.LightPosition = LightPosition;
    }
}

public sealed class BlinnPhongRenderer : PhongRenderer
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        FragmentShader.Blinn = true;
    }
}