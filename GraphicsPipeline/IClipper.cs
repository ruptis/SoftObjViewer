namespace GraphicsPipeline;

public interface IClipper<T> where T : unmanaged
{
    public int ClipTriangle(in Triangle<T> triangle, ref Span<Triangle<T>> clippedTriangles);
}
