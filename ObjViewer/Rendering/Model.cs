namespace ObjViewer.Rendering;

public readonly record struct Model(
    Mesh Mesh, 
    Transform Transform, 
    Texture? DiffuseMap = null,
    Texture? NormalMap = null, 
    Texture? SpecularMap = null)
{
    public Model() : this(new Mesh(), Transform.Identity) { }
    public Model(Mesh mesh) : this(mesh, Transform.Identity) { }
}
