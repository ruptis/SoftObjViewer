using System.Numerics;
namespace Utils.MeshLoader;

public class ObjParser : IMeshLoader
{
    private struct Face
    {
        public int VertexIndex { get; init; }
        public int NormalIndex { get; init; }
        public int TextureCoordsIndex { get; init; }
    }

    private readonly Dictionary<string, Action<string[]>> _parsers;

    private readonly List<Vector3> _vertices = [];
    private readonly List<Vector2> _textureCoords = [];
    private readonly List<Vector3> _normals = [];
    private readonly List<Face> _faces = [];

    public ObjParser()
    {
        _parsers = new Dictionary<string, Action<string[]>>
        {
            {
                "v", ParseVertices
            },
            {
                "vt", ParseTextureCoords
            },
            {
                "vn", ParseNormals
            },
            {
                "f", ParseFaces
            }
        };
    }

    public async Task<Mesh> LoadMeshAsync(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        var lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            var tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (_parsers.TryGetValue(tokens[0], out var parseAction))
            {
                parseAction(tokens);
            }
        }

        var tangents = new Vector3[_vertices.Count];
        for (var i = 0; i < _faces.Count; i += 3)
        {
            Vector3 tangent = CalculateTangent(_faces[i], _faces[i + 1], _faces[i + 2]);
            tangents[_faces[i].VertexIndex - 1] += tangent;
            tangents[_faces[i + 1].VertexIndex - 1] += tangent;
            tangents[_faces[i + 2].VertexIndex - 1] += tangent;
        }

        var vertices = new List<Vertex>();
        foreach (Face face in _faces)
        {
            var vertex = new Vertex
            {
                Normal = face.NormalIndex > 0 ? _normals[face.NormalIndex - 1] : Vector3.Zero,
                Position = face.VertexIndex > 0 ? _vertices[face.VertexIndex - 1] : Vector3.Zero,
                TextureCoordinates = face.TextureCoordsIndex > 0
                    ? _textureCoords[face.TextureCoordsIndex - 1]
                    : Vector2.Zero,
                Tangent = tangents[face.VertexIndex - 1],
            };
            vertices.Add(vertex);
        }

        var uniqueVertices = vertices.Distinct().ToList();
        var uniqueIndices = vertices.Select(v => uniqueVertices.IndexOf(v)).ToList();

        return new Mesh(uniqueVertices, uniqueIndices);
    }

    private Vector3 CalculateTangent(Face f0, Face f1, Face f2)
    {
        Vector3 edge1 = _vertices[f1.VertexIndex - 1] - _vertices[f0.VertexIndex - 1];
        Vector3 edge2 = _vertices[f2.VertexIndex - 1] - _vertices[f0.VertexIndex - 1];
        Vector2 deltaUv1 = _textureCoords[f1.TextureCoordsIndex - 1] - _textureCoords[f0.TextureCoordsIndex - 1];
        Vector2 deltaUv2 = _textureCoords[f2.TextureCoordsIndex - 1] - _textureCoords[f0.TextureCoordsIndex - 1];

        var f = 1.0f / (deltaUv1.X * deltaUv2.Y - deltaUv2.X * deltaUv1.Y);
        Vector3 tangent = Vector3.Normalize((deltaUv2.Y * edge1 - deltaUv1.Y * edge2) * f);

        return tangent;
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
