using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public class TexturedPhongRenderer : SceneRenderer<
    PhongShaderInput,
    PhongVertexShader,
    TexturedPhongFragmentShader,
    ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>,
    Clipper<PhongShaderInput, PhongLinearInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);

    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        VertexShader.Model = scene.Model.Transform.WorldMatrix;
        VertexShader.Mvp = scene.Model.Transform.WorldMatrix * scene.Camera.ViewMatrix * scene.Camera.ProjectionMatrix;

        VertexShader.ViewPosition = scene.Camera.Transform.Position;
        VertexShader.LightPosition = LightPosition;

        FragmentShader.DiffuseMap = scene.Model.DiffuseMap;
        FragmentShader.NormalMap = scene.Model.NormalMap;
        FragmentShader.SpecularMap = scene.Model.SpecularMap;
    }
}
public sealed class TexturedBlinnPhongRenderer : TexturedPhongRenderer
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        base.OnRenderScene(in scene, renderTarget);
        FragmentShader.Blinn = true;
    }
}
