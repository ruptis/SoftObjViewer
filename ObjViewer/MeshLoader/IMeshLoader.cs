using System.Threading.Tasks;
using ObjViewer.Rendering;
namespace ObjViewer.MeshLoader;

public interface IMeshLoader
{
    Task<Mesh> LoadMeshAsync(string path);
}
