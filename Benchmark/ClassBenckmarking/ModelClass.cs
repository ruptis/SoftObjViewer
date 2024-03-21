using System.Numerics;
using Utils;
namespace Benchmark.ClassBenckmarking;

public sealed class TextureClass(int width, int height, Vector3[] data)
{
    public static TextureClass Empty { get; } = new(0, 0, Array.Empty<Vector3>());

    public ref Vector3 SampleColor(in Vector2 uv)
    {
        var index = GetSampleIndex(uv);
        return ref data[index];
    }

    public Vector3 SampleNormal(in Vector2 uv)
    {
        var index = GetSampleIndex(uv);
        return data[index] * 2.0f - Vector3.One;
    }

    private int GetSampleIndex(in Vector2 uv)
    {
        var x = (int)(uv.X * width) % width;
        var y = (int)(uv.Y * height) % height;
        return y * width + x;
    }
}
public record MeshClass(IReadOnlyList<Vertex> Vertices, IReadOnlyList<int> Indices)
{
    public MeshClass() : this(new List<Vertex>(), new List<int>()) {}
}
public record ModelClassWithMeshClassAndTextureClass(
    MeshClass Mesh, 
    Transform Transform, 
    TextureClass? DiffuseMap, 
    TextureClass? NormalMap, 
    TextureClass? SpecularMap);

public record ModelClassWithMeshClass(
    MeshClass Mesh, 
    Transform Transform, 
    Texture? DiffuseMap, 
    Texture? NormalMap, 
    Texture? SpecularMap);

public readonly record struct ModelWithTextureClass(
    Mesh Mesh,
    Transform Transform, 
    TextureClass? DiffuseMap, 
    TextureClass? NormalMap, 
    TextureClass? SpecularMap);

public readonly record struct Model(
    Mesh Mesh,
    Transform Transform,
    Texture? DiffuseMap = null,
    Texture? NormalMap = null,
    Texture? SpecularMap = null);