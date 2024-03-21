using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Simple;

public sealed class LambertFragmentShader : IFragmentShader<Vertex>
{
    public Vector3 LightPosition { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        Vector3 lightDirection = Vector3.Normalize(LightPosition - input.Position);
        var lightIntensity = Math.Max(Vector3.Dot(input.Normal, lightDirection), 0.0f);
        color = new Vector4(lightIntensity, lightIntensity, lightIntensity, 1.0f);
    }
}
