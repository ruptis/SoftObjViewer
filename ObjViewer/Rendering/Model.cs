namespace ObjViewer.Rendering;

public readonly record struct Model(Mesh Mesh, Transform Transform)
{
    public Model() : this(new Mesh(), Transform.Identity) { }
    public Model(Mesh mesh) : this(mesh, Transform.Identity) { }
}
