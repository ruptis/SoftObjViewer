using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class SimpleVertexShader : IVertexShader<Vertex, Vertex>
{
    public Matrix4x4 Model { get; set; }
    public Matrix4x4 Mvp { get; set; }

    public void ProcessVertex(in Vertex input, out Vertex output, out Vector4 position)
    {
        position = Vector4.Transform(new Vector4(input.Position, 1.0f), Mvp);
        
        Vector3 normal = Vector3.Normalize(Vector3.TransformNormal(input.Normal, Model));

        Vector3 worldPosition = Vector3.Transform(input.Position, Model);

        output = input with
        {
            Position = worldPosition,
            Normal = normal,
        };
    }
}
