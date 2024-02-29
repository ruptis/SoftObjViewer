using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics;
namespace GraphicsPipeline;

public sealed class GraphicsPipeline<TVIn, TFIn> where TVIn : unmanaged where TFIn : unmanaged
{
    private readonly IVertexShader<TVIn, TFIn> _vertexShader;
    private readonly IFragmentShader<TFIn> _fragmentShader;
    private readonly IRasterizer<TFIn> _rasterizer;
    private readonly IClipper<TFIn> _clipper;

    private readonly Action<int> _cachedRenderCallback;
    private readonly Action<Fragment<TFIn>> _cachedFragmentCallback;

    private Viewport _viewport;

    private IRenderTarget _renderTarget = null!;
    private IReadOnlyList<TVIn> _vertices = null!;
    private IReadOnlyList<int> _indices = null!;

    public GraphicsPipeline(
        IVertexShader<TVIn, TFIn> vertexShader,
        IFragmentShader<TFIn> fragmentShader,
        IRasterizer<TFIn> rasterizer,
        IClipper<TFIn> clipper)
    {
        _vertexShader = vertexShader;
        _fragmentShader = fragmentShader;
        _rasterizer = rasterizer;
        _clipper = clipper;
        _cachedRenderCallback = RenderTriangle;
        _cachedFragmentCallback = ProcessFragment;
    }

    public bool BackfaceCullingEnabled { get; set; } = true;
    public bool ScissorTestEnabled { get; set; }

    public void Render(IReadOnlyList<TVIn> vertices, IReadOnlyList<int> indices, IRenderTarget renderTarget)
    {
        _renderTarget = renderTarget;
        _vertices = vertices;
        _indices = indices;

        _viewport.Set(renderTarget.Width, renderTarget.Height);

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

        if (BackfaceCullingEnabled && CullTriangle(in position0, in position1, in position2))
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

        Span<Triangle<TFIn>> triangles = stackalloc Triangle<TFIn>[6];

        var count = _clipper.ClipTriangle(in triangle, ref triangles);

        for (var i = 0; i < count; i++)
        {
            ToScreenSpace(ref triangles[i]);
            _rasterizer.Rasterize(in triangles[i], _cachedFragmentCallback);
        }
    }

    private void ProcessFragment(Fragment<TFIn> fragment)
    {
        if (ScissorTestEnabled && _viewport.IsOutOfViewport(in fragment.Position))
            return;

        if (!_renderTarget.ZTest(fragment.Position.X, fragment.Position.Y, fragment.Position.Z))
            return;

        _fragmentShader.ProcessFragment(in fragment.Position, in fragment.Data, out Color color);
        _renderTarget.SetPixel(fragment.Position.X, fragment.Position.Y, fragment.Position.Z, in color);
    }

    private void ToScreenSpace(ref Triangle<TFIn> triangle)
    {
        var wA = ToNdc(ref triangle.A);
        var wB = ToNdc(ref triangle.B);
        var wC = ToNdc(ref triangle.C);
        triangle.A = Vector4.Transform(triangle.A, _viewport.Matrix);
        triangle.B = Vector4.Transform(triangle.B, _viewport.Matrix);
        triangle.C = Vector4.Transform(triangle.C, _viewport.Matrix);

        triangle.A.W = wA;
        triangle.B.W = wB;
        triangle.C.W = wC;
    }

    private static float ToNdc(ref Vector4 position)
    {
        var w = 1.0f / position.W;
        position *= w;
        return w;
    }

    private static bool CullTriangle(in Vector4 position0, in Vector4 position1, in Vector4 position2)
    {
        Vector4 v0 = position1 - position0;
        Vector4 v1 = position2 - position0;
        Vector3 normal = Vector3.Cross(v0.AsVector128().AsVector3(), v1.AsVector128().AsVector3());
        return Vector3.Dot((-position0).AsVector128().AsVector3(), normal) >= 0;
    }
}
