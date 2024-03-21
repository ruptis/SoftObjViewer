using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ObjViewer.Rendering;
using ObjViewer.Rendering.Renderer;
using Utils;
using Utils.MeshLoader;
using Utils.TextureLoader;
using Transform = Utils.Transform;
namespace ObjViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly MainViewModel _viewModel = new();

    private ISceneRenderer Renderer => _viewModel.SelectedRenderMode.Renderer;
    private readonly IMeshLoader _meshLoader = new ObjParser();
    private readonly ITextureLoader _textureLoader = new PngTextureLoader();

    private Scene _scene;

    private readonly List<Light> _lights = [];
    private readonly Camera _camera;
    private readonly BitmapRenderTarget _renderTarget;

    private readonly Stopwatch _uiUpdateTimer = new();
    private readonly Stopwatch _frameTimer = new();
    private readonly Stopwatch _drawTimer = new();

    private Vector2 _previousMousePosition;
    private bool _isDragging;

    private readonly Action _presentAction;

    public MainWindow()
    {
        InitializeComponent();

        _renderTarget = new BitmapRenderTarget((int)Width, (int)Height);
        Image.Source = _renderTarget.Bitmap;

        _camera = new Camera(_renderTarget.Width / (float)_renderTarget.Height);
        _camera.Transform.Position = new Vector3(5, 3, 7);
        _camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

        /*_lights.Add(new Light(new Transform
            {
                Position = new Vector3(-3, 8, 8)
            },
            ColorUtils.Yellow.AsVector3(),
            LightType.Directional,
            5, 12, 45));

        _lights.Add(new Light(new Transform
            {
                Position = new Vector3(3, 8, 8)
            },
            ColorUtils.Red.AsVector3(),
            LightType.Directional,
            5, 12, 45));*/

        _lights.Add(new Light(new Transform
            {
                Position = new Vector3(0, 8, 8)
            },
            ColorUtils.White.AsVector3(),
            LightType.Directional,
            5, 12, 45));

        _scene = new Scene
        {
            Camera = _camera,
            Lights = _lights,
            Model = new Model()
        };

        Loaded += OnLoaded;

        DataContext = _viewModel;

        _uiUpdateTimer.Start();
        _frameTimer.Start();

        _presentAction = () => _renderTarget.Present();

        Task.Run(() =>
        {
            while (true)
                OnRendering();
        });
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _scene.Model = new Model
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

    private void OnRendering()
    {
        var frameTime = _frameTimer.Elapsed.TotalMilliseconds;
        _frameTimer.Restart();

        /*for (var i = 0; i < _lights.Count; i++)
        {
            _lights[i].Transform.RotateAround(Vector3.Zero, Vector3.UnitY, 0.08f);
            _lights[i].Transform.RotateAround(Vector3.Zero, Vector3.UnitX, 0.08f);
            _lights[i].Transform.LookAt(Vector3.Zero, Vector3.UnitY);
        }*/

        _drawTimer.Restart();
        Renderer.RenderScene(_scene, _renderTarget);
        Dispatcher.Invoke(_presentAction);
        _drawTimer.Stop();

        UpdateInfo(_drawTimer.Elapsed.TotalMilliseconds, frameTime);
    }

    private void UpdateInfo(double drawTime, double frameTime)
    {
        if (_uiUpdateTimer.ElapsedMilliseconds < 100)
            return;

        _viewModel.DrawTime = drawTime;
        _viewModel.FrameTime = frameTime;
        _viewModel.Fps = 1000.0 / frameTime;
        _viewModel.VertexCount = _scene.Model.Mesh.Vertices.Count;
        _viewModel.TriangleCount = _scene.Model.Mesh.Indices.Count / 3;

        _uiUpdateTimer.Restart();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.ChangedButton != MouseButton.Left)
            return;

        _isDragging = true;

        _previousMousePosition = ToVector(e.GetPosition(this));
    }


    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.ChangedButton != MouseButton.Left)
            return;

        _isDragging = false;

        _previousMousePosition = default;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_isDragging)
            return;

        Vector2 currentMousePosition = ToVector(e.GetPosition(this));
        Vector2 delta = currentMousePosition - _previousMousePosition;
        _previousMousePosition = currentMousePosition;

        var angleX = -delta.X * 0.01f;
        var angleY = -delta.Y * 0.01f;

        _camera.Transform.RotateAround(Vector3.Zero, Vector3.UnitY, angleX);
        _camera.Transform.RotateAround(Vector3.Zero, _camera.Transform.Right, angleY);
        _camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        var delta = e.Delta / 120.0f;

        _camera.Transform.Position += _camera.Transform.Forward * -delta;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.OemPlus)
            _lights[0].Intensity += 0.1f;
        else if (e.Key == Key.OemMinus)
            _lights[0].Intensity -= 0.1f;
    }

    private static Vector2 ToVector(Point point) => new((float)point.X, (float)point.Y);
}
