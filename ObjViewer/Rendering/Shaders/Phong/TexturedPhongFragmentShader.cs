using System;
using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders.Phong;

public sealed class TexturedPhongFragmentShader : IFragmentShader<PhongShaderData>
{
    private static readonly Vector3 AmbientColor = new(0.2f);
    private static readonly Vector3 LightColor = new(1.0f);
    private const float Shininess = 64.0f;
    
    public Texture NormalMap { get; set; }
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }
    
    public bool Blinn { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in PhongShaderData input, out Color color)
    {
        Vector3 normal = NormalMap.SampleNormal(input.TextureCoordinates);
        Vector3 diffuseSample = DiffuseMap.SampleColor(input.TextureCoordinates);
        Vector3 specularSample = SpecularMap.SampleColor(input.TextureCoordinates);
        
        var lambertian = Vector3.Dot(input.LightDirection, normal);
        
        Vector3 ambientComponent = AmbientColor * diffuseSample;
        Vector3 diffuseComponent = Math.Max(lambertian, 0.0f) * diffuseSample * LightColor;
        Vector3 specularComponent = Vector3.Zero;
        
        if (lambertian > 0.0f)
        {
            float specular;
            
            if (Blinn)
            {
                Vector3 halfVector = Vector3.Normalize(input.LightDirection + input.ViewDirection);
                specular = MathF.Pow(Math.Max(Vector3.Dot(halfVector, normal), 0.0f), Shininess);
            }
            else
            {
                Vector3 reflectDirection = Vector3.Reflect(-input.LightDirection, normal);
                specular = MathF.Pow(Math.Max(Vector3.Dot(reflectDirection, input.ViewDirection), 0.0f), Shininess);
            }
            
            specularComponent = specular * specularSample * LightColor;
        }
        
        Vector3 finalColor = ambientComponent + diffuseComponent + specularComponent;
        finalColor = Vector3.Clamp(finalColor, Vector3.Zero, Vector3.One);
        color = Color.FromArgb(
            (byte) (finalColor.X * 255),
            (byte) (finalColor.Y * 255),
            (byte) (finalColor.Z * 255)
        );
    }
}
