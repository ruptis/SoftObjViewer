using System.Threading.Tasks;
namespace ObjViewer.Rendering.TextureLoader;

public interface ITextureLoader
{
    Task<Texture> LoadTextureAsync(string path);
}