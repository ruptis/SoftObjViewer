using System.Threading.Tasks;
using GraphicsPipeline.Components;
namespace ObjViewer.Rendering.TextureLoader;

public interface ITextureLoader
{
    Task<Texture> LoadTextureAsync(string path, bool flipY = false);
}