using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace Utils.TextureLoader;

public class PngTextureLoader : ITextureLoader
{
    public async Task<Texture> LoadTextureAsync(string path, bool flipY = false)
    {
        await using FileStream file = File.OpenRead(path);

        var image = await Image.LoadAsync<Rgba32>(file);
        var data = new Vector3[image.Width * image.Height];
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var targetIndex = ((image.Height - y - 1) * image.Width + x);
                Rgba32 pixel = image[x, y];
                var red = pixel.R;
                var green = flipY ? (byte)(255 - pixel.G) : pixel.G;
                var blue = pixel.B;
                data[targetIndex] = new Vector3(red, green, blue) / 255.0f;
            }
        }
        return new Texture(image.Width, image.Height, data);
    }
}
