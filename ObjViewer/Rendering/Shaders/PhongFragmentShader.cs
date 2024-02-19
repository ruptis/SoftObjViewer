using System;
using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class PhongFragmentShader : IFragmentShader<Vertex>
{
    private static readonly Vector3 AmbientColor = new(0.2f, 0.2f, 0.2f);
    private static readonly Vector3 LightColor = new(1.0f, 1.0f, 1.0f);
    private const float Ambient = 0.1f;
    private const float Diffuse = 0.5f;
    private const float Specular = 0.5f;
    private const float Shininess = 32.0f;
    public Vector3 ViewPosition { get; set; }
    public Vector3 LightPosition { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        Vector3 lightDirection = Vector3.Normalize(LightPosition - input.Position);
        Vector3 viewDirection = Vector3.Normalize(ViewPosition - input.Position);
        Vector3 normal = Vector3.Normalize(input.Normal);

        Vector3 ambientComponent = Ambient * AmbientColor;
        Vector3 diffuseComponent = Diffuse * LightColor * Math.Max(Vector3.Dot(normal, lightDirection), 0.0f);
        Vector3 specularComponent = Vector3.Zero;

        if (Vector3.Dot(normal, lightDirection) > 0.0f)
        {
            Vector3 halfVector = Vector3.Normalize(lightDirection + viewDirection);
            var specularAngle = Math.Max(Vector3.Dot(normal, halfVector), 0.0f);
            specularComponent = Specular * LightColor * MathF.Pow(specularAngle, Shininess);
        }

        Vector3 finalColor = ambientComponent + diffuseComponent + specularComponent;
        finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);
        color = Color.FromArgb((int)(finalColor.X * 255), (int)(finalColor.Y * 255), (int)(finalColor.Z * 255));
    }
}
