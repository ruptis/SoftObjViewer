﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ObjViewer.Renderer;
using Utils.Components;
using Utils.Utils;
using Transform = Utils.Components.Transform;
namespace ObjViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Task<Model> _modelTask;
    private readonly MainViewModel _viewModel = new();

    private ISceneRenderer Renderer => _viewModel.SelectedRenderMode.Renderer;

    private Scene _scene;

    private readonly List<LightSource> _lights = [];
    private readonly Camera _camera;
    private readonly BitmapRenderTarget _renderTarget;

    private readonly Stopwatch _uiUpdateTimer = new();
    private readonly Stopwatch _frameTimer = new();
    private readonly Stopwatch _drawTimer = new();

    private Vector2 _previousMousePosition;
    private bool _isDragging;

    private bool _gizmosEnabled = true;

    private readonly Action _presentAction;

    public MainWindow(Task<Model> modelTask)
    {
        _modelTask = modelTask;
        InitializeComponent();

        _renderTarget = new BitmapRenderTarget((int)Image.Width, (int)Image.Height);
        Image.Source = _renderTarget.Bitmap;

        _camera = new Camera(_renderTarget.Width / (float)_renderTarget.Height);
        _camera.Transform.Position = new Vector3(0, 5, 8);
        _camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

        var directionalLight = new LightSource(new Transform
            {
                Position = new Vector3(0, 8, 8)
            },
            new Light(ColorUtils.White.AsVector3(),
                LightType.Directional,
                1, 20, 45));
        directionalLight.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

        _lights.Add(directionalLight);
        var pointLight = new LightSource(new Transform
            {
                Position = new Vector3(0, 5, 0)
            },
            new Light(ColorUtils.White.AsVector3(),
                LightType.Point,
                1, 20, 45));
        
        _lights.Add(pointLight);

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

    private async void OnLoaded(object sender, RoutedEventArgs e) => 
        _scene.Model = await _modelTask;

    private void OnRendering()
    {
        var frameTime = _frameTimer.Elapsed.TotalMilliseconds;
        _frameTimer.Restart();

        Renderer.GizmosEnabled = _gizmosEnabled;

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
        if (e.ChangedButton != MouseButton.Left || !Equals(e.Source, Image))
            return;

        Vector2 mousePosition = ToVector(e.GetPosition(Image));
        _previousMousePosition = mousePosition;

        Ray ray = _camera.ScreenPointToRay(mousePosition, new Vector2(_renderTarget.Width, _renderTarget.Height));

        LightSource? selectedLight = null;

        for (var i = 0; i < _lights.Count; i++)
        {
            LightSource light = _lights[i];
            if (light.BoundingBox.Intersects(ray))
            {
                selectedLight = light;
                break;
            }
        }
        
        _viewModel.SelectedLight = selectedLight;
        _isDragging = true;
    }


    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.ChangedButton != MouseButton.Left || !Equals(e.Source, Image))
            return;

        _previousMousePosition = default;

        _isDragging = false;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!Equals(e.Source, Image))
            return;

        Vector2 currentMousePosition = ToVector(e.GetPosition(Image));
        Vector2 delta = currentMousePosition - _previousMousePosition;
        _previousMousePosition = currentMousePosition;
        
        LightSource? selectedLight = _viewModel.SelectedLight;

        if (selectedLight != null && _isDragging)
        {
            Ray ray = _camera.ScreenPointToRay(currentMousePosition, new Vector2(_renderTarget.Width, _renderTarget.Height));
            var distance = Vector3.Distance(selectedLight.Transform.Position, _camera.Transform.Position);

            switch (selectedLight.Light.Type)
            {
                case LightType.Point:
                    selectedLight.Transform.Position = ray.Origin + ray.Direction * distance;
                    break;
                case LightType.Directional:
                    selectedLight.Transform.RotateAround(Vector3.Zero, Vector3.UnitY, delta.X * 0.01f);
                    selectedLight.Transform.RotateAround(Vector3.Zero, selectedLight.Transform.Right, delta.Y * 0.01f);
                    selectedLight.Transform.LookAt(Vector3.Zero, Vector3.UnitY);
                    break;
                case LightType.Spot:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            selectedLight.BoundingBox.Center = selectedLight.Transform.Position;
            return;
        }

        if (!_isDragging)
            return;

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
       if (e.Key == Key.G)
            _gizmosEnabled = !_gizmosEnabled;
    }

    private static Vector2 ToVector(Point point) => new((float)point.X, (float)point.Y);
}
