using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace ObjViewer.Rendering.TextureLoader;

public class PngTextureLoader : ITextureLoader
{
    public async Task<Texture> LoadTextureAsync(string path)
    {
        await using FileStream file = File.OpenRead(path);
        
        var image = await Image.LoadAsync<Rgba32>(file);
        var data = new byte[image.Width * image.Height * 3];
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var targetIndex = ((image.Height - y - 1) * image.Width + x) * 3;
                Rgba32 pixel = image[x, y];
                data[targetIndex] = pixel.R;
                data[targetIndex + 1] = pixel.G;
                data[targetIndex + 2] = pixel.B;
            }
        }
        return new Texture(image.Width, image.Height, data);
    }
}
