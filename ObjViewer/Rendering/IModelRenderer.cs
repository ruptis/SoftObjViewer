using System.Drawing;
using GraphicsPipeline;
namespace ObjViewer.Rendering;

public interface IModelRenderer
{
    Color ClearColor { get; set; }
    void DrawModel(in Model model, in Camera camera, in IRenderTarget renderTarget);
}
