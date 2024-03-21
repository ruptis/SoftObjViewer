using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.MultiLightPhong;

public sealed class MultiLightPhongVertexShader : IVertexShader<Vertex, MultiLightPhongShaderInput>
{
    public Matrix4x4 Model { get; set; }
    public Matrix4x4 Mvp { get; set; }

    public void ProcessVertex(in Vertex input, out MultiLightPhongShaderInput output, out Vector4 position)
    {
        position = Vector4.Transform(new Vector4(input.Position, 1.0f), Mvp);
        
        Vector3 worldPosition = Vector3.Transform(input.Position, Model);

        Vector3 normal = Vector3.Normalize(Vector3.TransformNormal(input.Normal, Model));
        Vector3 tangent = Vector3.Normalize(Vector3.TransformNormal(input.Tangent, Model));
        tangent = Vector3.Normalize(tangent - normal * Vector3.Dot(normal, tangent));
        Vector3 bitangent = Vector3.Normalize(Vector3.Cross(normal, tangent));

        output = new MultiLightPhongShaderInput
        {
            Position = worldPosition,
            Normal = normal,
            Tangent = tangent,
            Bitangent = bitangent,
            TextureCoordinates = input.TextureCoordinates
        };
    }
}