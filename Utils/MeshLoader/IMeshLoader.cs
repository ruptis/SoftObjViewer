namespace Utils.MeshLoader;

public interface IMeshLoader
{
    Task<Mesh> LoadMeshAsync(string path);
}
