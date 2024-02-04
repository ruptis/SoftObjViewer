using System.Numerics;
namespace GraphicsPipeline;

public record struct Fragment<T> where T : struct
{
    public Vector3 Position;
    public T Data;
}
