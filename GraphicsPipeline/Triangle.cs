using System.Numerics;
namespace GraphicsPipeline;

public readonly struct Triangle<T> where T : struct
{
    public Vector4 A { get; init; }
    public Vector4 B { get; init; }
    public Vector4 C { get; init; }
    public T AData { get; init; }
    public T BData { get; init; }
    public T CData { get; init; }
}
