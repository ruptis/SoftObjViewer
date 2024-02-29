using System.Drawing;
using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Phong;

public sealed class TexturedPhongFragmentShader : IFragmentShader<PhongShaderInput>
{
    private static readonly Vector3 AmbientColor = new(0.2f);
    private static readonly Vector3 LightColor = new(1.0f);
    private const float Shininess = 64.0f;

    public Texture NormalMap { get; set; }
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }

    public bool Blinn { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in PhongShaderInput input2, out Color color)
    {
        Vector3 normal = NormalMap.SampleNormal(input2.TextureCoordinates);
        Vector3 diffuseSample = DiffuseMap.SampleColor(input2.TextureCoordinates);
        Vector3 specularSample = SpecularMap.SampleColor(input2.TextureCoordinates);

        var lambertian = Vector3.Dot(input2.LightDirection, normal);

        Vector3 ambientComponent = AmbientColor * diffuseSample;
        Vector3 diffuseComponent = Math.Max(lambertian, 0.0f) * diffuseSample * LightColor;
        Vector3 specularComponent = Vector3.Zero;

        if (lambertian > 0.0f)
        {
            float specular;

            if (Blinn)
            {
                Vector3 halfVector = Vector3.Normalize(input2.LightDirection + input2.ViewDirection);
                specular = MathF.Pow(Math.Max(Vector3.Dot(halfVector, normal), 0.0f), Shininess);
            }
            else
            {
                Vector3 reflectDirection = Vector3.Reflect(-input2.LightDirection, normal);
                specular = MathF.Pow(Math.Max(Vector3.Dot(reflectDirection, input2.ViewDirection), 0.0f), Shininess);
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
