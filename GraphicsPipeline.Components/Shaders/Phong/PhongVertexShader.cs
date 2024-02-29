using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Phong;

public sealed class PhongVertexShader : IVertexShader<Vertex, PhongShaderInput>
{
    public Matrix4x4 Model { get; set; }
    public Matrix4x4 Mvp { get; set; }

    public Vector3 LightPosition { get; set; }
    public Vector3 ViewPosition { get; set; }

    public void ProcessVertex(in Vertex input, out PhongShaderInput output, out Vector4 position)
    {
        position = Vector4.Transform(new Vector4(input.Position, 1.0f), Mvp);

        Vector3 worldPosition = Vector3.Transform(input.Position, Model);

        Vector3 normal = Vector3.Normalize(Vector3.TransformNormal(input.Normal, Model));
        Vector3 tangent = Vector3.Normalize(Vector3.TransformNormal(input.Tangent, Model));
        tangent = Vector3.Normalize(tangent - normal * Vector3.Dot(normal, tangent));
        Vector3 bitangent = Vector3.Normalize(Vector3.Cross(normal, tangent));

        Matrix4x4 tbn = Matrix4x4.Transpose(new Matrix4x4(
            tangent.X, tangent.Y, tangent.Z, 0.0f,
            bitangent.X, bitangent.Y, bitangent.Z, 0.0f,
            normal.X, normal.Y, normal.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f
        ));

        Vector3 lightDirection = Vector3.Normalize(LightPosition - worldPosition);
        Vector3 viewDirection = Vector3.Normalize(ViewPosition - worldPosition);

        output = new PhongShaderInput
        {
            LightDirection = Vector3.TransformNormal(lightDirection, tbn),
            ViewDirection = Vector3.TransformNormal(viewDirection, tbn),
            TextureCoordinates = input.TextureCoordinates
        };
    }
}
