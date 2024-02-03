using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering;

public sealed class VertexShader : IVertexShader<Vertex, Vertex>
{
    public Matrix4x4 Model { get; set; }
    public Matrix4x4 View { get; set; }
    public Matrix4x4 Projection { get; set; }
    
    public void ProcessVertex(in Vertex input, out Vertex output, out Vector4 position)
    {
        position = new Vector4(input.Position, 1.0f);
        position = Vector4.Transform(position, Model);
        position = Vector4.Transform(position, View);
        position = Vector4.Transform(position, Projection);
        position /= position.W;
        
        output = input;
    }
}
