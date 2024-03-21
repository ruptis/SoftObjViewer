using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Simple;

public sealed class PhongFragmentShader : IFragmentShader<Vertex>
{
    private static readonly Vector3 AmbientColor = new(0.2f);
    private static readonly Vector3 LightColor = new(1.0f);
    private const float Ambient = 0.1f;
    private const float Diffuse = 0.5f;
    private const float Specular = 0.5f;
    private const float Shininess = 32.0f;
    public Vector3 ViewPosition { get; set; }
    public Vector3 LightPosition { get; set; }

    public bool Blinn { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Vector4 color)
    {
        Vector3 lightDirection = Vector3.Normalize(LightPosition - input.Position);
        Vector3 viewDirection = Vector3.Normalize(ViewPosition - input.Position);
        Vector3 normal = input.Normal;

        var lambertian = Vector3.Dot(normal, lightDirection);

        Vector3 ambientComponent = Ambient * AmbientColor;
        Vector3 diffuseComponent = Diffuse * Math.Max(lambertian, 0.0f) * LightColor;
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

            specularComponent = Specular * specular * LightColor;
        }

        Vector3 finalColor = ambientComponent + diffuseComponent + specularComponent;
        finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);
        color = new Vector4(finalColor, 1.0f);
    }
}
