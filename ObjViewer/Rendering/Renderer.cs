using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering;

public class Renderer
{
    private readonly GraphicsPipeline<Vertex, Vertex> _graphicsPipeline;

    private readonly VertexShader _vertexShader = new();
    private readonly FragmentShader _fragmentShader = new();
    private readonly TriangleRasterizer _bresenhamRasterizer = new();

    public Renderer()
    {
        _graphicsPipeline = new GraphicsPipeline<Vertex, Vertex>(
            _vertexShader,
            _fragmentShader,
            _bresenhamRasterizer
        );
    }

    public IRenderTarget? RenderTarget { get; set; }

    public Matrix4x4 Model { set => _vertexShader.Model = value; }
    public Matrix4x4 View { set => _vertexShader.View = value; }
    public Matrix4x4 Projection { set => _vertexShader.Projection = value; }

    public void DrawMesh(in Mesh mesh)
    {
        if (RenderTarget is null)
            return;
        
        RenderTarget.Clear(Color.SlateGray);
        _graphicsPipeline.Render(mesh.Vertices, mesh.Indices, RenderTarget);
        RenderTarget.Present();
    }
}
