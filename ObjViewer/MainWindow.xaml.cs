﻿using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ObjViewer.MeshLoader;
using ObjViewer.Rendering;
using ObjViewer.Rendering.Renderer;
using ObjViewer.Rendering.TextureLoader;
using Camera = ObjViewer.Rendering.Camera;
using Transform = ObjViewer.Rendering.Transform;
namespace ObjViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly MainViewModel _viewModel = new();

    private IModelRenderer Renderer => _viewModel.SelectedRenderMode.Renderer;
    private readonly IMeshLoader _meshLoader = new ObjParser();
    private readonly ITextureLoader _textureLoader = new PngTextureLoader();

    private Model _model = new();

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

        var bitmap = new WriteableBitmap(
            (int)Width,
            (int)Height,
            96,
            96,
            PixelFormats.Bgra32,
            null);
        Image.Source = bitmap;
        _renderTarget = new BitmapRenderTarget(bitmap);

        _camera = new Camera(_renderTarget.Width / (float)_renderTarget.Height);
        _camera.Transform.Position = new Vector3(0, 2, 8);
        _camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

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
        _model = new Model
        {
            Mesh = await _meshLoader.LoadMeshAsync("ModelSamples/chest2.obj"),
            DiffuseMap = await _textureLoader.LoadTextureAsync("ModelSamples/textures/chest_diffuse2.png"),
            NormalMap = await _textureLoader.LoadTextureAsync("ModelSamples/textures/KittyChest_low_normal.png", true),
            SpecularMap = await _textureLoader.LoadTextureAsync("ModelSamples/textures/chest_specular3.png"),
            Transform = new Transform
            {
                Position = new Vector3(0f, -2f, 0f),
                Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0),
                Scale = new Vector3(2f)
            }
        };
    }

    private void OnRendering()
    {
        var frameTime = _frameTimer.Elapsed.TotalMilliseconds;
        _frameTimer.Restart();

        _drawTimer.Restart();
        Renderer.DrawModel(_model, _camera, _renderTarget);
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
        _viewModel.VertexCount = _model.Mesh.Vertices.Count;
        _viewModel.TriangleCount = _model.Mesh.Indices.Count / 3;

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

    private static Vector2 ToVector(Point point) => new((float)point.X, (float)point.Y);
}
