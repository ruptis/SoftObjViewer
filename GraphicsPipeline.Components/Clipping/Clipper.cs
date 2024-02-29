using System.Numerics;
namespace GraphicsPipeline.Components.Clipping;

public sealed class Clipper<T, TInterpolator> : IClipper<T>
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

        var count = 3;
        var totalVertices = 3;
        for (var i = 0; i < _planes.Length; i++)
        {
            var vertexCount = count;

            if (count < 2)
                break;

            outVertices.CopyTo(inVertices);
            count = 0;

            ref Vector4 plane = ref _planes[i];
            var cull = true;

            for (var j = 0; j < vertexCount; j++)
            {
                planeTests[j] = Vector4.Dot(plane, vertices[inVertices[j]].Position);
                insideTests[j] = planeTests[j] >= 0.0f;
                cull &= !insideTests[j];
            }

            if (cull)
                break;

            for (var j = 0; j < vertexCount; j++)
            {
                var next = (j + 1) % vertexCount;

                var first = inVertices[j];
                var second = inVertices[next];

                var firstInside = insideTests[j];
                var secondInside = insideTests[next];

                if (firstInside != secondInside)
                {
                    var t = IntersectEdgePlane(planeTests[j], planeTests[next]);
                    T interpolated = _interpolator.Interpolate(in vertices[first].Data, in vertices[second].Data, t);
                    Vector4 interpolatedPosition = Vector4.Lerp(vertices[first].Position, vertices[second].Position, t);
                    vertices[totalVertices] = new Vertex(interpolatedPosition, interpolated);
                    outVertices[count++] = totalVertices++;
                    if (!firstInside)
                        outVertices[count++] = second;
                }
                else if (firstInside && secondInside)
                {
                    outVertices[count++] = second;
                }
            }
        }

        if (count < 3)
            return 0;

        var triangleCount = 0;
        for (var i = 1; i < count - 1; i++)
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

    private static float IntersectEdgePlane(float p1, float p2) =>
        -p1 / (p2 - p1);
}
