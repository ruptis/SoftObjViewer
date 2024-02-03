using System.Numerics;
namespace GraphicsPipeline;

public interface IVertexShader<TIn, TOut> where TIn : struct where TOut : struct
{
    public void ProcessVertex(in TIn input, out TOut output, out Vector4 position);
}