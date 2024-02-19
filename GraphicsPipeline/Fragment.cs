using System.Numerics;
namespace GraphicsPipeline;

public record struct Fragment<T> where T : struct
{
    public Vector4 Position;
    public T Data;
}
