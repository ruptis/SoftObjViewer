namespace Utils.Components;

public readonly record struct Model(
    Mesh Mesh,
    Transform Transform)
{
    private readonly Texture[] _textures = [Texture.Empty, Texture.Empty, Texture.Empty, Texture.Empty];

    public Model() : this(new Mesh(), Transform.Identity) {}
    public Model(Mesh mesh) : this(mesh, Transform.Identity) {}

    private enum PhongTextureSet
    {
        Diffuse = 0,
        Normal = 1,
        Specular = 2
    }

    private enum PbrTextureSet
    {
        Albedo = 0,
        Normal = 1,
        Rma = 2,
        Emissive = 3
    }

    public Texture DiffuseMap
    {
        get => _textures[(int)PhongTextureSet.Diffuse];
        init => _textures[(int)PhongTextureSet.Diffuse] = value;
    }
    
    public Texture NormalMap
    {
        get => _textures[(int)PhongTextureSet.Normal];
        init => _textures[(int)PhongTextureSet.Normal] = value;
    }
    
    public Texture SpecularMap
    {
        get => _textures[(int)PhongTextureSet.Specular];
        init => _textures[(int)PhongTextureSet.Specular] = value;
    }
    
    public Texture AlbedoTexture
    {
        get => _textures[(int)PbrTextureSet.Albedo];
        init => _textures[(int)PbrTextureSet.Albedo] = value;
    }
    
    public Texture NormalTexture
    {
        get => _textures[(int)PbrTextureSet.Normal];
        init => _textures[(int)PbrTextureSet.Normal] = value;
    }
    
    public Texture RmaTexture
    {
        get => _textures[(int)PbrTextureSet.Rma];
        init => _textures[(int)PbrTextureSet.Rma] = value;
    }
    
    public Texture EmissiveTexture
    {
        get => _textures[(int)PbrTextureSet.Emissive];
        init => _textures[(int)PbrTextureSet.Emissive] = value;
    }
}
