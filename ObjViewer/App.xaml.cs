using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using Utils.Components;
using Utils.MeshLoader;
using Utils.TextureLoader;
namespace ObjViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IMeshLoader _meshLoader = new ObjParser();
        private readonly ITextureLoader _textureLoader = new PngTextureLoader();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow(LoadModel());
            mainWindow.Show();
        }
        
        private async Task<Model> LoadModel()
        {
            return new Model
            {
                Mesh = await _meshLoader.LoadMeshAsync("ModelSamples/chest2.obj"),
                Transform = new Transform
                {
                    Position = new Vector3(0f, -2f, 0f),
                    Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0),
                    Scale = new Vector3(2f)
                },
                /*DiffuseMap = await _textureLoader.LoadTextureAsync("ModelSamples/textures/chest_diffuse2.png"),
                NormalMap = await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_normal.png", true),
                SpecularMap = await _textureLoader.LoadTextureAsync("ModelSamples/textures/chest_specular3.png")*/
                AlbedoTexture = await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_basecolor.png"),
                NormalTexture = await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_normal.png", true),
                RmaTexture = Texture.CreateRmaTexture(
                    await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_roughness.png"),
                    await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_metalic.png"),
                    await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_ao.png"))
            };
        }
    }
}
