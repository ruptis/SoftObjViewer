using System.Numerics;
using Utils;
using Utils.Components;
namespace GraphicsPipeline.Components.Shaders.Simple;

public sealed class SimpleVertexShader : IVertexShader<Vertex, Vertex>
{
    public Matrix4x4 Model { get; set; }
    public Matrix4x4 Mvp { get; set; }

    public void ProcessVertex(in Vertex input, out Vertex output, out Vector4 position)
    {
        position = Vector4.Transform(new Vector4(input.Position, 1.0f), Mvp);

        Vector3 worldPosition = Vector3.Transform(input.Position, Model);

        Vector3 normal = Vector3.Normalize(Vector3.TransformNormal(input.Normal, Model));

        output = input with
        {
            Position = worldPosition,
            Normal = normal
        };
    }
}
