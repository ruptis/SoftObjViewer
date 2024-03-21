using System.Numerics;
namespace GraphicsPipeline;

public interface IPostProcessor
{
    public void ProcessColor(ref Vector4 color);
}
