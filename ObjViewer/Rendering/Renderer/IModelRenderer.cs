using GraphicsPipeline;
using Utils;
namespace ObjViewer.Rendering.Renderer;

public interface IModelRenderer
{
    void DrawModel(in Model model, in Camera camera, IRenderTarget renderTarget);
}
