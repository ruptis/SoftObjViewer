using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders.Phong;

public sealed class PhongVertexShader : IVertexShader<Vertex, PhongShaderData>
{
    public Matrix4x4 ModelView { get; set; }
    public Matrix4x4 NormalMatrix { get; set; }
    public Matrix4x4 Mvp { get; set; }
    
    public Vector3 LightPosition { get; set; }
    public Vector3 ViewPosition { get; set; }
    
    public void ProcessVertex(in Vertex input, out PhongShaderData output, out Vector4 position)
    {
        position = Vector4.Transform(new Vector4(input.Position, 1.0f), Mvp);
        
        Vector3 normal = Vector3.Normalize(Vector3.TransformNormal(input.Normal, NormalMatrix));
        Vector3 tangent = Vector3.Normalize(Vector3.TransformNormal(input.Tangent, NormalMatrix));
        Vector3 bitangent = Vector3.Normalize(Vector3.Cross(normal, tangent));
        
        var toTangentSpace = new Matrix4x4(
            tangent.X, bitangent.X, normal.X, 0.0f,
            tangent.Y, bitangent.Y, normal.Y, 0.0f,
            tangent.Z, bitangent.Z, normal.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f
        );
        
        Vector3 pos = Vector3.Transform(input.Position, ModelView);
        Vector3 lightDirection = Vector3.Normalize(Vector3.Transform(LightPosition - pos, toTangentSpace));
        Vector3 viewDirection = Vector3.Normalize(Vector3.Transform(ViewPosition - pos, toTangentSpace));
        
        output = new PhongShaderData(lightDirection, viewDirection, input.TextureCoordinates);
    }
}