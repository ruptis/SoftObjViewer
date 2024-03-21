using System.Numerics;
namespace GraphicsPipeline;

public interface IVertexShader<TIn, TOut> where TIn : unmanaged where TOut : unmanaged
{
    public void ProcessVertex(in TIn input, out TOut output, out Vector4 position);
}
