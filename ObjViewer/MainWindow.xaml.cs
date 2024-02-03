using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ObjViewer.Rendering;
using Transform = ObjViewer.Rendering.Transform;
namespace ObjViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Camera _camera;
        private readonly Renderer _renderer;
        private readonly Model _model;

        private readonly Stopwatch _uiUpdateTimer = new();
        private readonly Stopwatch _frameTimer = new();
        private readonly Stopwatch _drawTimer = new();
        
        private Vector2 _previousMousePosition;
        private bool _isDragging;

        public MainWindow()
        {
            InitializeComponent();
            var bitmap = new WriteableBitmap(
                1600,
                900,
                96,
                96,
                PixelFormats.Bgra32,
                null);
            
            _renderer = new Renderer
            {
                RenderTarget = new BitmapRenderTarget(bitmap)
            };
            
            Image.Source = bitmap;

            _camera = new Camera(bitmap.PixelWidth / (float)bitmap.PixelHeight);
            _camera.Transform.Position = new Vector3(0, 2, 8);
            _camera.Transform.LookAt(Vector3.Zero, Vector3.UnitY);

            Mesh mesh = MeshGenerator.CreatePlane(5, 300);
            _model = new Model
            {
                Mesh = mesh,
                Transform = new Transform
                {
                    Position = Vector3.Zero,
                    Scale = new Vector3(1, 1, 1)
                }
            };
            
            _uiUpdateTimer.Start();
            _frameTimer.Start();
            CompositionTarget.Rendering += Render;
        }

        private void Render(object? sender, EventArgs eventArgs)
        {
            var frameTime = _frameTimer.Elapsed.TotalSeconds;
            _frameTimer.Restart();

            _renderer.Model = _model.Transform.WorldMatrix;
            _renderer.View = _camera.ViewMatrix;
            _renderer.Projection = _camera.ProjectionMatrix;
            
            _drawTimer.Restart();
            _renderer.DrawMesh(_model.Mesh);
            _drawTimer.Stop();
            
            UpdateUI(_drawTimer.Elapsed.TotalSeconds, frameTime);
        }
        
        private void UpdateUI(double drawTime, double frameTime)
        {
            if (_uiUpdateTimer.ElapsedMilliseconds < 100)
                return;

            TextBlock.Text = $"\nDraw time: {drawTime * 1000:F2} ms";
            TextBlock.Text += $"\nFrame time: {frameTime * 1000:F2} ms";
            TextBlock.Text += $"\nFPS: {1 / frameTime:F2}";
            TextBlock.Text += $"\nVertices: {_model.Mesh.Vertices.Count}";
            TextBlock.Text += $"\nTriangles: {_model.Mesh.Indices.Count / 3}";

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
            var angleY = delta.Y * 0.01f;

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
}
