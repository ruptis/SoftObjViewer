using System.Collections.Generic;
namespace ObjViewer.Rendering;

public readonly record struct Mesh(IReadOnlyList<Vertex> Vertices, IReadOnlyList<int> Indices);
