using System.Drawing;
using System.Numerics;
namespace GraphicsPipeline;

public interface IFragmentShader<TIn> where TIn : struct
{
    public void ProcessFragment(in Vector4 fragCoord, in TIn input, out Color color);
}
