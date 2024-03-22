using System.Numerics;
using Utils.Components;
using Utils.Utils;
namespace GraphicsPipeline.Components.Shaders.Pbr;

public sealed class PbrFragmentShader : IFragmentShader<PbrShaderInput>
{
    private static readonly Vector3 SpecularCoefficient = new(0.04f);

    public IReadOnlyList<LightSource> Lights { get; set; } = null!;
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
        Vector3 albedoSample = AlbedoTexture.SampleColor(input.TextureCoordinates).SrgbToLinear();
        Vector3 rmaSample = RmaTexture.SampleColor(input.TextureCoordinates);

        Vector3 surfaceColor = ShadeSurface(
            in position, in viewDirection, in normal,
            Lights,
            in albedoSample,
            rmaSample.X, rmaSample.Y, rmaSample.Z);

        surfaceColor += EmissiveTexture?.SampleColor(input.TextureCoordinates) ?? Vector3.Zero;

        color = new Vector4(surfaceColor, 1.0f);
    }

    private static Vector3 ShadeSurface(
        in Vector3 position, in Vector3 viewDirection, in Vector3 normal,
        IReadOnlyList<LightSource> lights,
        in Vector3 albedo,
        float roughness, float metallic, float ambientOcclusion)
    {
        var nDotV = Math.Max(Vector3.Dot(normal, viewDirection), 0.0f);

        var alpha = roughness * roughness;

        Vector3 cDiffuse = Vector3.Lerp(albedo, Vector3.Zero, metallic) * ambientOcclusion;
        Vector3 cSpecular = Vector3.Lerp(SpecularCoefficient, albedo, metallic) * ambientOcclusion;

        Vector3 color = Vector3.Zero;

        for (var i = 0; i < lights.Count; i++)
        {
            GetDirectionAndRadiance(lights[i], in position, out Vector3 lightDirection, out Vector3 radiance);

            Vector3 halfVector = Vector3.Normalize(lightDirection + viewDirection);

            var nDotL = Math.Max(Vector3.Dot(normal, lightDirection), 0.0f);
            var lDotH = Math.Max(Vector3.Dot(lightDirection, halfVector), 0.0f);
            var nDotH = Math.Max(Vector3.Dot(normal, halfVector), 0.0f);

            var alpha2 = alpha * alpha;

            Vector3 diffuse = BRDF.RenormalizedDisneyDiffuse(in cDiffuse, nDotL, nDotV, lDotH, roughness);
            Vector3 specular = BRDF.SpecularBRDF(in cSpecular, lDotH, nDotH, alpha2);

            color += nDotL * radiance * (diffuse + specular);
        }

        return color;
    }

    private static void GetDirectionAndRadiance(LightSource light, in Vector3 position, out Vector3 lightDirection, out Vector3 radiance)
    {
        var attenuation = 1.0f;

        switch (light.Light.Type)
        {
            case LightType.Directional:
                lightDirection = light.Transform.Forward;
                break;
            case LightType.Point:
                Vector3 toLight = light.Transform.Position - position;
                var distanceSquared = toLight.LengthSquared();
                lightDirection = toLight * MathF.ReciprocalSqrtEstimate(distanceSquared);
                attenuation = MathUtils.Square(
                    MathUtils.Clamp01(1.0f - MathUtils.Square(distanceSquared * MathUtils.Square(light.Light.InvRange))));
                break;
            case LightType.Spot:
                Vector3 toSpot = light.Transform.Position - position;
                var distanceSquaredSpot = toSpot.LengthSquared();
                lightDirection = toSpot * MathF.ReciprocalSqrtEstimate(distanceSquaredSpot);
                var spotFactor = Vector3.Dot(-lightDirection, light.Transform.Forward);
                const float deg2Rad = MathF.PI / 180.0f;
                var spotAngleCos = MathF.Cos(light.Light.SpotAngle * deg2Rad);
                attenuation = MathUtils.Square(
                    MathUtils.Clamp01(1.0f - MathUtils.Square(distanceSquaredSpot * MathUtils.Square(light.Light.InvRange))) *
                    MathUtils.Clamp01((spotFactor - spotAngleCos) / (1.0f - spotAngleCos)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        radiance = light.Light.Color * light.Light.Intensity * attenuation;
    }
}
