using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Pbr;
using GraphicsPipeline.Components.Shaders.PostProcess;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public sealed class PbrRenderer() : SceneRenderer<
    PbrShaderInput,
    PbrVertexShader,
    PbrFragmentShader,
    ScanlineTriangleRasterizer<PbrShaderInput, PbrScanlineInterpolator>,
    Clipper<PbrShaderInput, PbrLinearInterpolator>>(new ToneMappingPostProcessor())
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        VertexShader.Model = scene.Model.Transform.WorldMatrix;
        VertexShader.Mvp = scene.Model.Transform.WorldMatrix * scene.Camera.ViewMatrix * scene.Camera.ProjectionMatrix;

        FragmentShader.Lights = scene.Lights;
        FragmentShader.ViewPosition = scene.Camera.Transform.Position;

        FragmentShader.AlbedoTexture = scene.Model.AlbedoTexture;
        FragmentShader.NormalTexture = scene.Model.NormalTexture;
        FragmentShader.RmaTexture = scene.Model.RmaTexture;
        FragmentShader.EmissiveTexture = scene.Model.EmissiveTexture;
    }
}
