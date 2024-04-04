using System.Numerics;
using Assimp;
using Utils.Components;
using Mesh = Utils.Components.Mesh;
using Scene = Assimp.Scene;
namespace Utils.MeshLoader;

public class AssimpLoader : IMeshLoader
{
    public Task<Mesh> LoadMeshAsync(string path)
    {
        return Task.Run(() =>
        {
            var importer = new AssimpContext();
            Scene? scene = importer.ImportFile(path,
                PostProcessSteps.Triangulate |
                PostProcessSteps.OptimizeGraph |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.GenerateSmoothNormals |
                PostProcessSteps.CalculateTangentSpace);
            if (scene is null)
                throw new Exception();

            var meshes = new Mesh[scene.MeshCount];
            for (var meshNumber = 0; meshNumber < scene.MeshCount; meshNumber++)
            {
                Assimp.Mesh? meshData = scene.Meshes[meshNumber];
                var verticesPositions = meshData.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
                var indices = meshData.Faces.SelectMany(f => f.Indices).ToArray();

                var normals = meshData.HasNormals
                    ? meshData.Normals.Select(n => new Vector3(n.X, n.Y, n.Z)).ToArray()
                    : Enumerable.Repeat(Vector3.UnitZ, verticesPositions.Length).ToArray();

                var textureCoordinates = meshData.HasTextureCoords(0)
                    ? meshData.TextureCoordinateChannels[0].Select(t => new Vector2(t.X, t.Y)).ToArray()
                    : Enumerable.Repeat(Vector2.Zero, verticesPositions.Length).ToArray();

                var tangents = meshData.HasTangentBasis
                    ? meshData.Tangents.Select(t => new Vector3(t.X, t.Y, t.Z)).ToArray()
                    : Enumerable.Repeat(Vector3.UnitX, verticesPositions.Length).ToArray();

                var vertices = new Vertex[verticesPositions.Length];
                for (var i = 0; i < vertices.Length; i++)
                    vertices[i] = new Vertex(verticesPositions[i], normals[i], textureCoordinates[i], tangents[i]);

                meshes[meshNumber] = new Mesh(vertices, indices);
            }
            
            var combinedVertices = new List<Vertex>();
            var combinedIndices = new List<int>();
            
            foreach (var mesh in meshes)
            {
                var offset = combinedVertices.Count;
                combinedVertices.AddRange(mesh.Vertices);
                combinedIndices.AddRange(mesh.Indices.Select(i => i + offset));
            }

            return new Mesh(combinedVertices, combinedIndices);
        });
    }
}
