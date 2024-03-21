using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Pbr;

public sealed class PbrFragmentShader : IFragmentShader<PbrShaderInput>
{
    private static readonly Vector3 SpecularCoefficient = new(0.04f);
    
    public IReadOnlyList<Light> Lights { get; set; } = null!;
    public Vector3 ViewPosition { get; set; }
    
    public Texture AlbedoTexture { get; set; }
    public Texture NormalTexture { get; set; }
    public Texture RmaTexture { get; set; }
    public Texture? EmissiveTexture { get; set; }

    public void ProcessFragment(in Vector4 fragCoord, in PbrShaderInput input, out Vector4 color)
    {
        Vector3 position = input.Position;
        Vector3 viewDirection = Vector3.Normalize(ViewPosition - position);
        
        Matrix4x4 tbn = MathUtils.TbnSpace(input.Tangent, input.Bitangent, input.Normal);
        
        Vector3 normal = Vector3.TransformNormal(NormalTexture.SampleNormal(input.TextureCoordinates), tbn);
        Vector3 albedoSample = AlbedoTexture.SampleColor(input.TextureCoordinates);
        Vector3 rmaSample = RmaTexture.SampleColor(input.TextureCoordinates);
        
        Vector3 surfaceColor = ShadeSurface(in position, in viewDirection, in normal, Lights, in albedoSample, rmaSample.X, rmaSample.Y, rmaSample.Z);

        surfaceColor += EmissiveTexture?.SampleColor(input.TextureCoordinates) ?? Vector3.Zero;
        
        color = new Vector4(surfaceColor, 1.0f);
    }
    
    private static Vector3 ShadeSurface(
        in Vector3 position, in Vector3 viewDirection, in Vector3 normal, 
        IReadOnlyList<Light> lights, 
        in Vector3 albedo, float roughness, float metallic, float ambientOcclusion)
    {
        var nDotV = Math.Max(Vector3.Dot(normal, viewDirection), 0.0f);
        
        var alpha = roughness * roughness;

        Vector3 cDiffuse = Vector3.Lerp(albedo, Vector3.Zero, metallic) * ambientOcclusion;
        Vector3 cSpecular = Vector3.Lerp(SpecularCoefficient, albedo, metallic) * ambientOcclusion;
        
        Vector3 color = Vector3.Zero;

        for (var i = 0; i < lights.Count; i++)
        {
            Vector3 toLight = lights[i].Transform.Position - position;
            var distance = toLight.Length();
            Vector3 lightDirection = toLight / distance;

            var attenuation = 1.0f / (distance * distance);
            Vector3 radiance = lights[i].Color * lights[i].Intensity * attenuation;
            
            Vector3 halfVector = Vector3.Normalize(lightDirection + viewDirection);
            
            var nDotL = Math.Max(Vector3.Dot(normal, lightDirection), 0.0f);
            var lDotH = Math.Max(Vector3.Dot(lightDirection, halfVector), 0.0f);
            var nDotH = Math.Max(Vector3.Dot(normal, halfVector), 0.0f);

            var diffuseFactor = DiffuseBarley(nDotL, nDotV, lDotH, roughness);
            var specular = SpecularBRDF(alpha, in cSpecular, lDotH, nDotH);
            
            color += nDotL * radiance * (cDiffuse * diffuseFactor + specular);
        }
        
        return color;
    }
    
    private static float DiffuseBarley(float nDotL, float nDotV, float lDotH, float roughness)
    {
        var fd90 = new Vector3(0.5f + 2.0f * lDotH * lDotH * roughness);
        return FresnelSchlick(Vector3.Zero, fd90, nDotL).X * FresnelSchlick(Vector3.Zero, fd90, nDotV).X;
    }
    
    private static Vector3 FresnelSchlick(in Vector3 f0, in Vector3 fd90, float x) => 
        f0 + (fd90 - f0) * MathF.Pow(1.0f - x, 5.0f);

    private static Vector3 SpecularBRDF(float alpha, in Vector3 specularColor, float lDotH, float nDotH)
    {
        var specularD = SpecularDGgx(alpha, nDotH);
        Vector3 specularF = FresnelSchlick(in specularColor, Vector3.One, lDotH);
        var specularG = GShlickSmithHable(alpha, lDotH);

        return specularD * specularG * specularF;
    }
    
    private static float GShlickSmithHable(float alpha, float lDotH) => 
        MathF.ReciprocalEstimate(float.Lerp(lDotH * lDotH, 1.0f, alpha * alpha * 0.25f));

    private static float SpecularDGgx(float alpha, float nDotH)
    {
        var alpha2 = alpha * alpha;
        var lower = nDotH * nDotH * (alpha2 - 1.0f) + 1.0f;
        return alpha2 / MathF.Max(float.Epsilon, float.Pi * lower * lower);
    }
}
