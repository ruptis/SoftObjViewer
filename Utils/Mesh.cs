namespace Utils;

public readonly record struct Mesh(IReadOnlyList<Vertex> Vertices, IReadOnlyList<int> Indices)
{
    public Mesh() : this(new List<Vertex>(), new List<int>()) {}
}
