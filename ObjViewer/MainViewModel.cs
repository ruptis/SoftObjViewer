using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ObjViewer.Rendering.Renderer;
namespace ObjViewer;

public sealed class MainViewModel : INotifyPropertyChanged
{
    public record RenderMode(string Name, IModelRenderer Renderer);
    public MainViewModel()
    {
        RenderModes = new ObservableCollection<RenderMode>
        {
            new("Wireframe", new WireframeRenderer()),
            new("Wireframe Lambert", new WireframeLambertRenderer()),
            new("Flat Lambert", new FlatLambertRenderer()),
            new("Lambert", new LambertRenderer()),
            new("Phong", new PhongRenderer()),
            new("Normal", new NormalRenderer()),
            new("UV", new UvRenderer())
        };
        _selectedRenderMode = RenderModes[0];
    }
    
    public ObservableCollection<RenderMode> RenderModes { get; }

    private RenderMode _selectedRenderMode;
    public RenderMode SelectedRenderMode
    {
        get => _selectedRenderMode;
        set
        {
            if (_selectedRenderMode == value)
                return;
            
            _selectedRenderMode = value;
            OnPropertyChanged();
        }
    }
    
    private double _frameTime;
    public double FrameTime
    {
        get => _frameTime;
        set
        {
            if (IsEqual(_frameTime, value))
                return;
            
            _frameTime = value;
            OnPropertyChanged();
        }
    }
    
    private double _drawTime;
    public double DrawTime
    {
        get => _drawTime;
        set
        {
            if (IsEqual(_drawTime, value))
                return;
            
            _drawTime = value;
            OnPropertyChanged();
        }
    }
    
    private double _fps;
    public double Fps
    {
        get => _fps;
        set
        {
            if (IsEqual(_fps, value))
                return;
            
            _fps = value;
            OnPropertyChanged();
        }
    }
    
    private int _vertexCount;
    public int VertexCount
    {
        get => _vertexCount;
        set
        {
            if (_vertexCount == value)
                return;
            
            _vertexCount = value;
            OnPropertyChanged();
        }
    }
    
    private int _triangleCount;
    public int TriangleCount
    {
        get => _triangleCount;
        set
        {
            if (_triangleCount == value)
                return;
            
            _triangleCount = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private static bool IsEqual(double a, double b) => Math.Abs(a - b) < double.Epsilon;
}
