using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Phong;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public class TexturedPhongRenderer : ModelRenderer<
    PhongShaderInput,
    PhongVertexShader,
    TexturedPhongFragmentShader,
    ScanlineTriangleRasterizer<PhongShaderInput, PhongScanlineInterpolator>,
    Clipper<PhongShaderInput, PhongLinearInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 8, 8);

    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        VertexShader.Model = model.Transform.WorldMatrix;
        VertexShader.Mvp = model.Transform.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix;

        VertexShader.ViewPosition = camera.Transform.Position;
        VertexShader.LightPosition = LightPosition;

        FragmentShader.NormalMap = model.NormalMap ?? Texture.Checkerboard512;
        FragmentShader.DiffuseMap = model.DiffuseMap ?? Texture.Checkerboard512;
        FragmentShader.SpecularMap = model.SpecularMap ?? Texture.Checkerboard512;
    }
}
public sealed class TexturedBlinnPhongRenderer : TexturedPhongRenderer
{
    protected override void OnDraw(in Model model, in Camera camera, IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        FragmentShader.Blinn = true;
    }
}
