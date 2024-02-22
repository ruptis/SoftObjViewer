using System.Numerics;
using GraphicsPipeline;
using ObjViewer.Rendering.Rasterization;
using ObjViewer.Rendering.Rasterization.Interpolation;
using ObjViewer.Rendering.Shaders.Phong;
namespace ObjViewer.Rendering.Renderer;

public class TexturedPhongRenderer : ModelRenderer<PhongShaderData, PhongVertexShader, TexturedPhongFragmentShader, ScanlineTriangleRasterizer<PhongShaderData, PhongDataInterpolator>>
{
    private static readonly Vector3 LightPosition = new(0, 2, 8);
    
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        VertexShader.LightPosition = LightPosition;
        VertexShader.ViewPosition = camera.Transform.Position;
        
        VertexShader.ModelView = model.Transform.WorldMatrix * camera.ViewMatrix;
        VertexShader.Mvp = VertexShader.ModelView * camera.ProjectionMatrix;

        FragmentShader.NormalMap = model.NormalMap ?? Texture.Checkerboard512;
        FragmentShader.DiffuseMap = model.DiffuseMap ?? Texture.Checkerboard512;
        FragmentShader.SpecularMap = model.SpecularMap ?? Texture.Checkerboard512;
    }
}

public sealed class TexturedBlinnPhongRenderer : TexturedPhongRenderer
{
    protected override void OnDraw(in Model model, in Camera camera, in IRenderTarget renderTarget)
    {
        base.OnDraw(model, camera, renderTarget);
        FragmentShader.Blinn = true;
    }
}
