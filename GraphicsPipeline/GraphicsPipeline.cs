using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics;
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

    public bool BackfaceCullingEnabled { get; set; } = true;

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

        if (ClipTriangle(in position0, in position1, in position2) ||
            BackfaceCullingEnabled && CullTriangle(in position0, in position1, in position2))
            return;

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

    private static bool ClipTriangle(in Vector4 position0, in Vector4 position1, in Vector4 position2)
    {
        if (position0.W < 0 || position1.W < 0 || position2.W < 0)
            return true;

        if (position0.Z > position0.W || position1.Z > position1.W || position2.Z > position2.W)
            return true;

        if (position0.Z < 0 || position1.Z < 0 || position2.Z < 0)
            return true;

        if (position0.X < -position0.W && position1.X < -position1.W && position2.X < -position2.W)
            return true;

        if (position0.X > position0.W && position1.X > position1.W && position2.X > position2.W)
            return true;

        if (position0.Y < -position0.W && position1.Y < -position1.W && position2.Y < -position2.W)
            return true;

        return position0.Y > position0.W && position1.Y > position1.W && position2.Y > position2.W;
    }

    private static bool CullTriangle(in Vector4 position0, in Vector4 position1, in Vector4 position2)
    {
        Vector3 normal = Vector3.Cross((position1 - position0).AsVector128().AsVector3(), (position2 - position0).AsVector128().AsVector3());
        return normal.Z < 0;
    }
}
