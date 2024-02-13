using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
namespace ObjViewer.Rendering.MeshLoader;

public class MeshGenerator : IMeshLoader
{
    private static Mesh CreateCube(float size = 1f)
    {
        List<Vertex> vertices = [];
        List<int> triangles = [];

        var halfSize = size / 2;
        Vector3[] positions =
        [
            new Vector3(-halfSize, -halfSize, -halfSize),
            new Vector3(halfSize, -halfSize, -halfSize),
            new Vector3(halfSize, halfSize, -halfSize),
            new Vector3(-halfSize, halfSize, -halfSize),
            new Vector3(-halfSize, -halfSize, halfSize),
            new Vector3(halfSize, -halfSize, halfSize),
            new Vector3(halfSize, halfSize, halfSize),
            new Vector3(-halfSize, halfSize, halfSize)
        ];

        Vector3[] normals =
        [
            -Vector3.UnitZ,
            Vector3.UnitZ,
            -Vector3.UnitX,
            Vector3.UnitX,
            -Vector3.UnitY,
            Vector3.UnitY
        ];

        Vector2[] uvs =
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0)
        ];

        int[] indices =
        [
            0, 3, 2, 2, 1, 0, // Back
            4, 5, 6, 6, 7, 4, // Front
            0, 4, 7, 7, 3, 0, // Left
            1, 2, 6, 6, 5, 1, // Right
            0, 1, 5, 5, 4, 0,  // Bottom
            3, 7, 6, 6, 2, 3, // Top
        ];

        for (var i = 0; i < 6; i++)
        for (var j = 0; j < 6; j++)
            vertices.Add(new Vertex(positions[indices[i * 6 + j]], normals[i], uvs[j]));

        triangles.AddRange(Enumerable.Range(0, 36));

        return new Mesh(vertices, triangles);
    }

    private static Mesh CreateSphere(float radius = 1f, int segments = 16)
    {
        List<Vertex> vertices = [];
        List<int> triangles = [];

        for (var i = 0; i <= segments; i++)
        {
            var phi = MathF.PI * i / segments;
            for (var j = 0; j <= segments; j++)
            {
                var theta = MathF.PI * 2 * j / segments;
                var x = MathF.Cos(theta) * MathF.Sin(phi);
                var y = MathF.Cos(phi);
                var z = MathF.Sin(theta) * MathF.Sin(phi);
                vertices.Add(new Vertex(new Vector3(x, y, z) * radius, new Vector3(x, y, z), new Vector2(j / (float)segments, i / (float)segments)));
            }
        }

        for (var i = 0; i < segments; i++)
        {
            for (var j = 0; j < segments; j++)
            {
                var a = i * (segments + 1) + j;
                var b = a + segments + 1;
                triangles.Add(a);
                triangles.Add(a + 1);
                triangles.Add(b);
                triangles.Add(b);
                triangles.Add(a + 1);
                triangles.Add(b + 1);
            }
        }

        return new Mesh(vertices, triangles);
    }

    private static Mesh CreatePlane(float size = 1f, int segments = 1)
    {
        List<Vertex> vertices = [];
        List<int> triangles = [];

        for (var i = 0; i <= segments; i++)
        {
            for (var j = 0; j <= segments; j++)
            {
                var x = (j / (float)segments - 0.5f) * size;
                var z = (i / (float)segments - 0.5f) * size;
                vertices.Add(new Vertex(new Vector3(x, 0, z), Vector3.UnitY, new Vector2(j / (float)segments, i / (float)segments)));
            }
        }

        for (var i = 0; i < segments; i++)
        {
            for (var j = 0; j < segments; j++)
            {
                var a = i * (segments + 1) + j;
                var b = a + segments + 1;
                triangles.Add(a);
                triangles.Add(a + 1);
                triangles.Add(b);
                triangles.Add(b);
                triangles.Add(a + 1);
                triangles.Add(b + 1);
            }
        }

        return new Mesh(vertices, triangles);
    }
    public Task<Mesh> LoadMeshAsync(string path)
    {
        var parts = path.Split(' ');
        return parts[0].ToLower() switch
        {
            "cube" => Task.FromResult(CreateCube(parts.Length > 1 ? float.Parse(parts[1]) : 1)),
            "sphere" => Task.FromResult(CreateSphere(parts.Length > 1 ? float.Parse(parts[1]) : 1, parts.Length > 2 ? int.Parse(parts[2]) : 16)),
            "plane" => Task.FromResult(CreatePlane(parts.Length > 1 ? float.Parse(parts[1]) : 1, parts.Length > 2 ? int.Parse(parts[2]) : 1)),
            _ => throw new ArgumentException("Invalid mesh type")
        };
    }
}
