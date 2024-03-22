using System.Numerics;
using Utils;
namespace GraphicsPipeline.Components.Shaders.Pbr;

public static class BRDF
{
    public static Vector3 DiffuseBarley(in Vector3 diffuseColor, float nDotL, float nDotV, float lDotH, float roughness)
    {
        var fd90 = new Vector3(0.5f + 2.0f * lDotH * lDotH * roughness);
        return diffuseColor * FresnelSchlick(Vector3.One, fd90, nDotL).X * FresnelSchlick(Vector3.One, fd90, nDotV).X;
    }

    public static Vector3 RenormalizedDisneyDiffuse(in Vector3 diffuseColor, float nDotL, float nDotV, float lDotH, float roughness)
    {
        var energyBias = float.Lerp(0.0f, 0.5f, roughness);
        var energyFactor = float.Lerp(1.0f, 1.0f / 1.51f, roughness);
        var fd90 = new Vector3(energyBias + 2.0f * lDotH * lDotH * roughness);
        var f0 = new Vector3(1.0f, 1.0f, 1.0f);
        var lightScatter = FresnelSchlick(f0, fd90, nDotL).X;
        var viewScatter = FresnelSchlick(f0, fd90, nDotL).X;

        return diffuseColor * (lightScatter * viewScatter * energyFactor);
    }

    public static Vector3 DiffuseChan(in Vector3 diffuseColor, float nDotL, float nDotV, float lDotH, float nDotH, float alpha2)
    {
        nDotL = MathUtils.Clamp01(nDotL);
        nDotV = MathUtils.Clamp01(nDotV);
        lDotH = MathUtils.Clamp01(lDotH);
        nDotH = MathUtils.Clamp01(nDotH);

        var g = MathUtils.Clamp01(1.0f / 18.0f * MathF.Log2(2.0f * MathF.ReciprocalEstimate(alpha2) - 1.0f));

        var f0 = lDotH * MathUtils.Pow5(1.0f - lDotH);
        var fdV = 1.0f - 0.75f * MathUtils.Pow5(1.0f - nDotV);
        var fdL = 1.0f - 0.75f * MathUtils.Pow5(1.0f - nDotL);

        var fd = float.Lerp(f0, fdV * fdL, MathUtils.Clamp01(2.2f * g - 0.5f));

        var fb = ((34.5f * g - 59.0f) * g + 24.5f) *
            lDotH * float.Exp2(-MathF.Max(73.2f * g - 21.2f, 8.9f) * MathF.Sqrt(nDotH));

        return diffuseColor * (1.0f / float.Pi * (fd + fb));
    }

    public static Vector3 FresnelSchlick(in Vector3 f0, in Vector3 fd90, float x) =>
        f0 + (fd90 - f0) * MathF.Pow(1.0f - x, 5.0f);

    public static Vector3 SpecularBRDF(in Vector3 specularColor, float lDotH, float nDotH, float alpha2)
    {
        var specularD = SpecularDGgx(alpha2, nDotH);
        Vector3 specularF = FresnelSchlick(in specularColor, Vector3.One, lDotH);
        var specularG = GShlickSmithHable(alpha2, lDotH);

        return specularD * specularG * specularF;
    }

    public static float GShlickSmithHable(float alpha2, float lDotH) =>
        MathF.ReciprocalEstimate(float.Lerp(lDotH * lDotH, 1.0f, alpha2 * 0.25f));

    public static float SpecularDGgx(float alpha2, float nDotH)
    {
        var lower = nDotH * nDotH * (alpha2 - 1.0f) + 1.0f;
        return alpha2 / MathF.Max(float.Epsilon, float.Pi * lower * lower);
    }
}
