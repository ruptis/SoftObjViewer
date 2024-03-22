using Utils.Components;
namespace Utils.TextureLoader;

public interface ITextureLoader
{
    Task<Texture> LoadTextureAsync(string path, bool flipY = false);
}
