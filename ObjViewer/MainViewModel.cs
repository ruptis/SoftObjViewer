using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using ObjViewer.Renderer;
using Utils.Components;
using Utils.Utils;
namespace ObjViewer;

public sealed class MainViewModel : INotifyPropertyChanged
{
    public record RenderMode(string Name, ISceneRenderer Renderer);
    public MainViewModel()
    {
        RenderModes = new ObservableCollection<RenderMode>
        {
            new("Wireframe", new WireframeRenderer()),
            new("Wireframe Lambert", new WireframeLambertRenderer()),
            new("Flat Lambert", new FlatLambertRenderer()),
            new("Lambert", new LambertRenderer()),
            new("Phong", new PhongRenderer()),
            new("Blinn-Phong", new BlinnPhongRenderer()),
            new("Textured Phong", new TexturedPhongRenderer()),
            new("Textured Blinn-Phong", new TexturedBlinnPhongRenderer()),
            new("HDR Phong", new HdrPhongRenderer()),
            new("PBR", new PbrRenderer()),
            new("Normal", new NormalRenderer()),
            new("UV", new UvRenderer()),
            new("Depth", new DepthRenderer()),
        };
        _selectedRenderMode = RenderModes[0];
    }

    public ObservableCollection<RenderMode> RenderModes { get; }
    
    public ObservableCollection<LightType> LightTypes { get; } = new(Enum.GetValues<LightType>());
    
    public Visibility LightControlsVisibility => _selectedLight is not null ? Visibility.Visible : Visibility.Collapsed;
    public Color LightColor
    {
        get => ToColor(_selectedLight?.Light.Color ?? ColorUtils.White.AsVector3());
        set
        {
            if (_selectedLight is null)
                return;

            _selectedLight.Light.Color = ToVector3(value);
            OnPropertyChanged();
        }
    }
    public LightType LightType
    {
        get => _selectedLight?.Light.Type ?? LightType.Directional;
        set
        {
            if (_selectedLight is null)
                return;

            _selectedLight.Light.Type = value;
            OnPropertyChanged();
        }
    }
    public float LightIntensity
    {
        get => _selectedLight?.Light.Intensity ?? 1;
        set
        {
            if (_selectedLight is null)
                return;

            _selectedLight.Light.Intensity = value;
            OnPropertyChanged();
        }
    }
    public float LightRange
    {
        get => _selectedLight?.Light.Range ?? 20;
        set
        {
            if (_selectedLight is null)
                return;

            _selectedLight.Light.Range = value;
            OnPropertyChanged();
        }
    }
    public float LightSpotAngle
    {
        get => _selectedLight?.Light.SpotAngle ?? 45;
        set
        {
            if (_selectedLight is null)
                return;

            _selectedLight.Light.SpotAngle = value;
            OnPropertyChanged();
        }
    }
    
    private static Color ToColor(in Vector3 vector) => Color.FromRgb((byte)(vector.X * byte.MaxValue), (byte)(vector.Y * byte.MaxValue), (byte)(vector.Z * byte.MaxValue));
    private static Vector3 ToVector3(in Color color) => new(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue);
    
    private LightSource? _selectedLight;
    public LightSource? SelectedLight
    {
        get => _selectedLight;
        set
        {
            if (_selectedLight == value)
                return;

            _selectedLight = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LightControlsVisibility));
            OnPropertyChanged(nameof(LightColor));
            OnPropertyChanged(nameof(LightType));
            OnPropertyChanged(nameof(LightIntensity));
            OnPropertyChanged(nameof(LightRange));
            OnPropertyChanged(nameof(LightSpotAngle));
        }
    }

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
