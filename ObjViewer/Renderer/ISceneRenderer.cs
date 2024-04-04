using System.Numerics;
using GraphicsPipeline;
using Utils.Components;
namespace ObjViewer.Renderer;

public interface ISceneRenderer
{
    public void RenderScene(in Scene scene, IRenderTarget renderTarget);
    public Vector4 ClearColor { get; set; }
    public bool GizmosEnabled { get; set; }
}
