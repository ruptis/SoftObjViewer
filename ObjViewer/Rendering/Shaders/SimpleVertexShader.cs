using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class SimpleVertexShader : IVertexShader<Vertex, Vertex>
{
    public Matrix4x4 Model { get; set; }
    public Matrix4x4 Mvp { get; set; }

    public void ProcessVertex(in Vertex input, out Vertex output, out Vector4 position)
    {
        position = new Vector4(input.Position, 1.0f);
        position = Vector4.Transform(position, Mvp);
        var w = 1.0f / position.W;
        position /= position.W;

        Vector3 normal = Vector3.TransformNormal(input.Normal, Model);
        normal = Vector3.Normalize(normal);

        Vector3 worldPosition = Vector3.Transform(input.Position, Model);

        output = input with
        {
            Position = worldPosition,
            Normal = normal,
            W = w,
            TextureCoordinates = input.TextureCoordinates * w
        };
    }
}
