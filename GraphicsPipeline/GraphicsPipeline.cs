using System.Drawing;
using System.Numerics;
namespace GraphicsPipeline;

public class GraphicsPipeline<TVIn, TFIn> where TVIn : struct where TFIn : struct
{
    private readonly IVertexShader<TVIn, TFIn> _vertexShader;
    private readonly IFragmentShader<TFIn> _fragmentShader;
    private readonly IRasterizer<TFIn> _rasterizer;

    private readonly Action<Fragment<TFIn>> _cachedFragmentCallback;
    
    private IRenderTarget _renderTarget = null!;

    public GraphicsPipeline(IVertexShader<TVIn, TFIn> vertexShader, IFragmentShader<TFIn> fragmentShader, IRasterizer<TFIn> rasterizer)
    {
        _vertexShader = vertexShader;
        _fragmentShader = fragmentShader;
        _rasterizer = rasterizer;
        _cachedFragmentCallback = ProcessFragment;
    }

    public void Render(IReadOnlyList<TVIn> vertices, IReadOnlyList<int> indices, IRenderTarget renderTarget)
    {
        _renderTarget = renderTarget;
        _rasterizer.SetViewport(renderTarget.Width, renderTarget.Height);
        
        Parallel.For(0, indices.Count / 3, index => Render(vertices, indices, index));
        /*for (var index = 0; index < indices.Count / 3; index++)
            Render(vertices, indices, index);*/
    }
    private void Render(IReadOnlyList<TVIn> vertices, IReadOnlyList<int> indices, int index)
    {
        var i0 = indices[index * 3];
        var i1 = indices[index * 3 + 1];
        var i2 = indices[index * 3 + 2];
        TVIn v0 = vertices[i0];
        TVIn v1 = vertices[i1];
        TVIn v2 = vertices[i2];

        _vertexShader.ProcessVertex(in v0, out TFIn v0Out, out Vector4 position0);
        _vertexShader.ProcessVertex(in v1, out TFIn v1Out, out Vector4 position1);
        _vertexShader.ProcessVertex(in v2, out TFIn v2Out, out Vector4 position2);

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
        _renderTarget.DrawPixel((int)fragment.Position.X, (int)fragment.Position.Y, color);
    }
}
