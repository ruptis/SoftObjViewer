using System.Drawing;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Renderer;

public interface IModelRenderer
{
    Color ClearColor { get; set; }
    void DrawModel(in Model model, in Camera camera, IRenderTarget renderTarget);
}
