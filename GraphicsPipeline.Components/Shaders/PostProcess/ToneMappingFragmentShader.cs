using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.PostProcess;

public sealed class ToneMappingFragmentShader : IFragmentShader<Vector2>
{
    public Texture HdrBuffer { get; set; }
    
    public void ProcessFragment(in Vector4 fragCoord, in Vector2 input, out Vector4 color)
    {
        Vector3 hdrColor = HdrBuffer.SampleColor(input);
        
        color = new Vector4(hdrColor / (hdrColor + Vector3.One), 1.0f);
    }
}
