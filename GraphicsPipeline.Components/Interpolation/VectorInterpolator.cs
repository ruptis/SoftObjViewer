using System.Numerics;
namespace GraphicsPipeline.Components.Interpolation;

public sealed class VectorInterpolator : IScanlineInterpolator<Vector3>, ILinearInterpolator<Vector3>
{
    public Vector3 Average(in Vector3 v0, in Vector3 v1, in Vector3 v2) => (v0 + v1 + v2) / 3.0f;
    public Vector3 Interpolate(in Vector3 v0, in Vector3 v1, float w0, float w1, float w, float amount) => Vector3.Lerp(v0, v1, amount);
    public Vector3 Interpolate(in Vector3 v0, in Vector3 v1, float t) => Vector3.Lerp(v0, v1, t);
}
