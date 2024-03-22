using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.Debug;

public sealed class GizmoVertexShader : IVertexShader<Vector3, Vector3>
{
    public Matrix4x4 ViewProjection { get; set; }
    
    public void ProcessVertex(in Vector3 input, out Vector3 output, out Vector4 position)
    {
        position = Vector4.Transform(new Vector4(input, 1.0f), ViewProjection);
        output = input;
    }
}
