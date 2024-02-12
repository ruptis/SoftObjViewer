using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ObjViewer.Rendering.MeshLoader;

public class ObjParser : IMeshLoader
{
    private struct Face
    {
        public int VertexIndex { get; init; }
        public int NormalIndex { get; init; }
        public int TextureCoordsIndex { get; init; }
    }

    private readonly Dictionary<string, Action<string[]>> _parsers;

    private List<Vector3> _vertices = [];
    private List<Vector2> _textureCoords = [];
    private List<Vector3> _normals = [];
    private List<Face> _faces = [];
    private List<int> _vertexIndices = [];

    public ObjParser()
    {
        _parsers = new Dictionary<string, Action<string[]>>
        {
            { "v", ParseVertices },
            { "vt", ParseTextureCoords },
            { "vn", ParseNormals },
            { "f", ParseFaces }
        };
    }

    public async Task<Mesh> LoadMeshAsync(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        var lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            var tokens = line.Split(" ");
            if (_parsers.TryGetValue(tokens[0], out Action<string[]> parseAction))
            {
                parseAction(tokens);
            }
        }

        var vertices = new List<Vertex>();
        foreach (var face in _faces)
        {
            /*_vertexIndices.Add(face.VertexIndex > 0 ? face.VertexIndex - 1 : 0);*/
            var vertex = new Vertex
            {
                Normal = face.NormalIndex > 0 ? _normals[face.NormalIndex - 1] : Vector3.Zero,
                Position = face.VertexIndex > 0 ? _vertices[face.VertexIndex - 1] : Vector3.Zero,
                TextureCoordinates = face.TextureCoordsIndex > 0
                    ? _textureCoords[face.TextureCoordsIndex - 1]
                    : Vector2.Zero
            };
            vertices.Add(vertex);
        }

        _vertexIndices = [..Enumerable.Range(0, vertices.Count)];
        return new Mesh(vertices, _vertexIndices);
    }

    private void ParseVertices(string[] s)
    {
        _vertices.Add(new Vector3(
            float.Parse(s[1]),
            float.Parse(s[2]),
            float.Parse(s[3]))
        );
    }

    private void ParseTextureCoords(string[] s)
    {
        _textureCoords.Add(new Vector2(
                float.Parse(s[1]),
                s.Length > 2
                    ? float.Parse(s[2])
                    : 0.0f
            )
        );
    }

    private void ParseNormals(string[] s)
    {
        _normals.Add(new Vector3(float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3])));
    }

    private void ParseFaces(string[] s)
    {
        List<Face> indices = new();
        for (int i = 1; i < s.Length; i++)
        {
            string[] parts = s[i].Split("/");
            Face face = new()
            {
                VertexIndex = int.Parse(parts[0]),
                TextureCoordsIndex = parts.Length > 1 && parts[1] != string.Empty ? int.Parse(parts[1]) : 0,
                NormalIndex = parts.Length > 2 ? int.Parse(parts[2]) : 0
            };
            indices.Add(face);
        }

        if (s.Length == 4)
        {
            _faces.AddRange(indices);
        }
        else if (s.Length > 4)
        {
            for (int i = 0; i < indices.Count - 1; i++)
            {
                _faces.Add(indices[0]);
                _faces.Add(indices[i]);
                _faces.Add(indices[i + 1]);
            }
        }
    }
}