using System.Numerics;
namespace GraphicsPipeline;

public record struct Triangle<T> where T : struct
{
    public Vector4 A;
    public Vector4 B;
    public Vector4 C;
    public T AData;
    public T BData;
    public T CData;
}
