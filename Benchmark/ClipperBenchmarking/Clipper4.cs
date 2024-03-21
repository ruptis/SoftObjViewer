using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Interpolation;
namespace Benchmark.ClipperBenchmarking;

public sealed class Clipper4<T, TInterpolator> : IClipper<T>
    where T : unmanaged
    where TInterpolator : ILinearInterpolator<T>, new()
{
    private readonly Vector4[] _planes =
    [
        new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
        new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
        new Vector4(0.0f, -1.0f, 0.0f, 1.0f),
        new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
        new Vector4(0.0f, 0.0f, -1.0f, 1.0f)
    ];

    private readonly struct Vertex(Vector4 position, T data)
    {
        public readonly Vector4 Position = position;
        public readonly T Data = data;
    }

    private readonly TInterpolator _interpolator = new();

    public int ClipTriangle(in Triangle<T> triangle, ref Span<Triangle<T>> clippedTriangles)
    {
        Span<Vertex> vertices = stackalloc Vertex[20];
        Span<int> outVertices = stackalloc int[9];
        Span<int> inVertices = stackalloc int[9];
        Span<bool> insideTests = stackalloc bool[9];
        Span<float> planeTests = stackalloc float[9];

        vertices[0] = new Vertex(triangle.A, triangle.AData);
        vertices[1] = new Vertex(triangle.B, triangle.BData);
        vertices[2] = new Vertex(triangle.C, triangle.CData);
        outVertices[0] = 0;
        outVertices[1] = 1;
        outVertices[2] = 2;

        var outVerticesCount = 3;
        var totalVertices = 3;
        for (var i = 0; i < _planes.Length; i++)
        {
            if (outVerticesCount < 2)
                break;

            outVertices.CopyTo(inVertices);

            ref Vector4 plane = ref _planes[i];
            var fullOutside = true;
            var fullInside = true;

            for (var j = 0; j < outVerticesCount; j++)
            {
                planeTests[j] = Vector4.Dot(plane, vertices[inVertices[j]].Position);
                insideTests[j] = planeTests[j] >= 0.0f;
                fullOutside &= !insideTests[j];
                fullInside &= insideTests[j];
            }
            
            if (fullInside)
                continue;
            
            var inVerticesCount = outVerticesCount;
            outVerticesCount = 0;
            
            if (fullOutside)
                break;

            for (var j = 0; j < inVerticesCount; j++)
            {
                var next = (j + 1) % inVerticesCount;

                var secondIndex = inVertices[next];

                var firstInside = insideTests[j];
                var secondInside = insideTests[next];

                if (firstInside != secondInside)
                {
                    var t = -planeTests[j] / (planeTests[next] - planeTests[j]);
                    T interpolated = _interpolator.Interpolate(in vertices[inVertices[j]].Data, in vertices[inVertices[secondIndex]].Data, t);
                    Vector4 interpolatedPosition = Vector4.Lerp(vertices[inVertices[j]].Position, vertices[inVertices[secondIndex]].Position, t);
                    vertices[totalVertices] = new Vertex(interpolatedPosition, interpolated);
                    outVertices[outVerticesCount++] = totalVertices++;
                    
                    if (!firstInside)
                        outVertices[outVerticesCount++] = secondIndex;
                }
                else if (firstInside && secondInside)
                {
                    outVertices[outVerticesCount++] = secondIndex;
                }
            }
        }

        if (outVerticesCount < 3)
            return 0;

        if (totalVertices == 3)
        {
            clippedTriangles[0] = triangle;
            return 1;
        }

        var triangleCount = 0;
        for (var i = 1; i < outVerticesCount - 1; i++)
        {
            clippedTriangles[triangleCount++] = new Triangle<T>
            {
                A = vertices[outVertices[0]].Position,
                B = vertices[outVertices[i]].Position,
                C = vertices[outVertices[i + 1]].Position,
                AData = vertices[outVertices[0]].Data,
                BData = vertices[outVertices[i]].Data,
                CData = vertices[outVertices[i + 1]].Data
            };
        }

        return triangleCount;
    }
}
