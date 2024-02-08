using System.Drawing;
using System.Numerics;
namespace GraphicsPipeline;

public sealed class GraphicsPipeline<TVIn, TFIn> where TVIn : struct where TFIn : struct
{
    private readonly IVertexShader<TVIn, TFIn> _vertexShader;
    private readonly IFragmentShader<TFIn> _fragmentShader;
    private readonly IRasterizer<TFIn> _rasterizer;

    private readonly Action<int> _cachedRenderCallback;
    private readonly Action<Fragment<TFIn>> _cachedFragmentCallback;

    private IRenderTarget _renderTarget = null!;
    private IReadOnlyList<TVIn> _vertices = null!;
    private IReadOnlyList<int> _indices = null!;

    public GraphicsPipeline(IVertexShader<TVIn, TFIn> vertexShader, IFragmentShader<TFIn> fragmentShader, IRasterizer<TFIn> rasterizer)
    {
        _vertexShader = vertexShader;
        _fragmentShader = fragmentShader;
        _rasterizer = rasterizer;
        _cachedRenderCallback = RenderTriangle;
        _cachedFragmentCallback = ProcessFragment;
    }

    public void Render(IReadOnlyList<TVIn> vertices, IReadOnlyList<int> indices, IRenderTarget renderTarget)
    {
        _renderTarget = renderTarget;
        _vertices = vertices;
        _indices = indices;

        _rasterizer.SetViewport(renderTarget.Width, renderTarget.Height);

        Parallel.For(0, indices.Count / 3, _cachedRenderCallback);
    }

    private void RenderTriangle(int index)
    {
        var triangleIndex = index * 3;
        TVIn v0 = _vertices[_indices[triangleIndex]];
        TVIn v1 = _vertices[_indices[triangleIndex + 1]];
        TVIn v2 = _vertices[_indices[triangleIndex + 2]];

        _vertexShader.ProcessVertex(v0, out TFIn v0Out, out Vector4 position0);
        _vertexShader.ProcessVertex(v1, out TFIn v1Out, out Vector4 position1);
        _vertexShader.ProcessVertex(v2, out TFIn v2Out, out Vector4 position2);

        var triangle = new Triangle<TFIn>
        {
            A = position0,
            B = position1,
            C = position2,
            AData = v0Out,
            BData = v1Out,
            CData = v2Out
        };

        _rasterizer.Rasterize(in triangle, _cachedFragmentCallback);
    }

    private void ProcessFragment(Fragment<TFIn> fragment)
    {
        _fragmentShader.ProcessFragment(in fragment.Position, in fragment.Data, out Color color);
        _renderTarget.DrawPixel(fragment.Position.X, fragment.Position.Y, fragment.Position.Z, in color);
    }
}
