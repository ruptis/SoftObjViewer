using System.Threading.Tasks;
namespace ObjViewer.Rendering.MeshLoader;

public interface IMeshLoader
{
    Task<Mesh> LoadMeshAsync(string path);
}
