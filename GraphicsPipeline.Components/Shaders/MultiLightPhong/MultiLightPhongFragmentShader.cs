using System.Numerics;
using Utils;
using Utils.Components;
using Utils.Utils;
namespace GraphicsPipeline.Components.Shaders.MultiLightPhong;

public class MultiLightPhongFragmentShader : IFragmentShader<MultiLightPhongShaderInput>
{
    private static readonly Vector3 AmbientColor = new(0.2f);
    private const float Shininess = 64.0f;
    
    public IReadOnlyList<LightSource> Lights { get; set; } = null!;
    public Vector3 ViewPosition { get; set; }

    public Texture NormalMap { get; set; }
    public Texture DiffuseMap { get; set; }
    public Texture SpecularMap { get; set; }
    
    public void ProcessFragment(in Vector4 fragCoord, in MultiLightPhongShaderInput input, out Vector4 color)
    {
        Vector3 viewDirection = Vector3.Normalize(ViewPosition - input.Position);
        
        Matrix4x4 tbn = MathUtils.TbnSpace(input.Tangent, input.Bitangent, input.Normal);
        
        Vector3 normal = Vector3.TransformNormal(NormalMap.SampleNormal(input.TextureCoordinates), tbn);
        Vector3 diffuseSample = DiffuseMap.SampleColor(input.TextureCoordinates).SrgbToLinear();
        Vector3 specularSample = SpecularMap.SampleColor(input.TextureCoordinates);
        
        Vector3 ambientComponent = AmbientColor * diffuseSample;
        Vector3 lightingComponent = Vector3.Zero;

        for (var i = 0; i < Lights.Count; i++)
        {
            LightSource light = Lights[i];
            Vector3 toLight = light.Transform.Position - input.Position;
            var distance = toLight.Length();

            Vector3 lightDirection = toLight / distance;

            var attenuation = 1.0f / (1.0f + 0.07f * distance + 0.017f * distance * distance);
            attenuation *= attenuation;

            Vector3 radiance = light.Light.Color * light.Light.Intensity * attenuation;

            var lambertian = Vector3.Dot(lightDirection, normal);
            Vector3 diffuseComponent = Math.Max(lambertian, 0.0f) * diffuseSample * radiance;
            Vector3 specularComponent = Vector3.Zero;

            if (lambertian > 0.0f)
            {
                Vector3 halfVector = Vector3.Normalize(lightDirection + viewDirection);
                var specular = MathF.Pow(Math.Max(Vector3.Dot(halfVector, normal), 0.0f), Shininess);

                specularComponent = specular * specularSample * radiance;
            }

            lightingComponent += diffuseComponent + specularComponent;
        }

        Vector3 finalColor = ambientComponent + lightingComponent;
        color = new Vector4(finalColor, 1.0f);
    }
}
