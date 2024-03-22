using System.Numerics;
using Utils.Utils;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class GizmoFragmentShader : IFragmentShader<Vector3>
{
    public Vector4 Color { get; set; } = ColorUtils.Red;

    public void ProcessFragment(in Vector4 fragCoord, in Vector3 input, out Vector4 color)
    {
        color = Color;
    }
}
