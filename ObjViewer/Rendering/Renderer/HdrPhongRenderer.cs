using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.MultiLightPhong;
using GraphicsPipeline.Components.Shaders.PostProcess;
using Utils;
using Utils.Components;
namespace ObjViewer.Rendering.Renderer;

public class HdrPhongRenderer() : SceneRenderer<
    MultiLightPhongShaderInput,
    MultiLightPhongVertexShader,
    MultiLightPhongFragmentShader,
    ScanlineTriangleRasterizer<MultiLightPhongShaderInput, MultiLightPhongScanlineInterpolator>,
    Clipper<MultiLightPhongShaderInput, MultiLightPhongLinearInterpolator>>(new ToneMappingPostProcessor())
{
    protected override void OnRenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        VertexShader.Model = scene.Model.Transform.WorldMatrix;
        VertexShader.Mvp = scene.Model.Transform.WorldMatrix * scene.Camera.ViewMatrix * scene.Camera.ProjectionMatrix;
        
        FragmentShader.Lights = scene.Lights;
        FragmentShader.ViewPosition = scene.Camera.Transform.Position;

        FragmentShader.DiffuseMap = scene.Model.DiffuseMap;
        FragmentShader.NormalMap = scene.Model.NormalMap;
        FragmentShader.SpecularMap = scene.Model.SpecularMap;
    }
}
