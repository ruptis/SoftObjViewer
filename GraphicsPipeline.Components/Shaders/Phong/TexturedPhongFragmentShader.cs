using System.Drawing;
using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.Phong;

public sealed class TexturedPhongFragmentShader : IFragmentShader<PhongShaderInput>
{
    private static readonly Vector3 AmbientColor = new(0.2f);
    private static readonly Vector3 LightColor = new(1.0f);
    private const float Shininess = 64.0f;

    public Texture NormalMap { get; set; }
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }

    public Vector3 LightPosition { get; set; }
    public Vector3 ViewPosition { get; set; }

    public bool Blinn { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in PhongShaderInput input, out Color color)
    {
        var tbn = new Matrix4x4(
            input.Tangent.X, input.Tangent.Y, input.Tangent.Z, 0.0f,
            input.Bitangent.X, input.Bitangent.Y, input.Bitangent.Z, 0.0f,
            input.Normal.X, input.Normal.Y, input.Normal.Z, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f
        );

        Vector3 lightDirection = Vector3.Normalize(LightPosition - input.Position);
        Vector3 viewDirection = Vector3.Normalize(ViewPosition - input.Position);

        Vector3 normal = NormalMap.SampleNormal(input.TextureCoordinates);
        normal = Vector3.TransformNormal(normal, tbn);
        Vector3 diffuseSample = DiffuseMap.SampleColor(input.TextureCoordinates);
        Vector3 specularSample = SpecularMap.SampleColor(input.TextureCoordinates);

        var lambertian = Vector3.Dot(lightDirection, normal);

        Vector3 ambientComponent = AmbientColor * diffuseSample;
        Vector3 diffuseComponent = Math.Max(lambertian, 0.0f) * diffuseSample * LightColor;
        Vector3 specularComponent = Vector3.Zero;

        if (lambertian > 0.0f)
        {
            float specular;

            if (Blinn)
            {
                Vector3 halfVector = Vector3.Normalize(lightDirection + viewDirection);
                specular = MathF.Pow(Math.Max(Vector3.Dot(halfVector, normal), 0.0f), Shininess);
            }
            else
            {
                Vector3 reflectDirection = Vector3.Reflect(-lightDirection, normal);
                specular = MathF.Pow(Math.Max(Vector3.Dot(reflectDirection, viewDirection), 0.0f), Shininess);
            }

            specularComponent = specular * specularSample * LightColor;
        }

        Vector3 finalColor = ambientComponent + diffuseComponent + specularComponent;
        finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);
        color = Color.FromArgb(
            (byte)(finalColor.X * 255),
            (byte)(finalColor.Y * 255),
            (byte)(finalColor.Z * 255)
        );
    }
}
