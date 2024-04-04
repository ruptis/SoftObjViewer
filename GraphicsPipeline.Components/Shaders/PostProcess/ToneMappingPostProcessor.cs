using System.Numerics;
using Utils.Utils;
namespace GraphicsPipeline.Components.Shaders.PostProcess;

public sealed class ToneMappingPostProcessor : IPostProcessor
{
    private static readonly Vector4 A = new(2.51f);
    private static readonly Vector4 B = new(0.03f);
    private static readonly Vector4 C = new(2.43f);
    private static readonly Vector4 D = new(0.59f);
    private static readonly Vector4 E = new(0.14f);
    
    public float Exposure { get; set; } = 1.0f;
    
    public void ProcessColor(ref Vector4 color)
    {
        var w = color.W;
        color *= Exposure;

        color = color * (A * color + B) / (color * (C * color + D) + E);

        color.LinearToSrgb();
        color.W = w;
    }
}
