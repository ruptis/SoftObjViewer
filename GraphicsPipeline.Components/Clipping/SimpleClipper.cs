namespace GraphicsPipeline.Components.Clipping;

public sealed class SimpleClipper<T> : IClipper<T> where T : unmanaged
{
    public int ClipTriangle(in Triangle<T> triangle, ref Span<Triangle<T>> clippedTriangles)
    {
        if (triangle.A.Z < -triangle.A.W && triangle.B.Z < -triangle.B.W && triangle.C.Z < -triangle.C.W)
            return 0;

        if (triangle.A.Z > triangle.A.W && triangle.B.Z > triangle.B.W && triangle.C.Z > triangle.C.W)
            return 0;

        if (triangle.A.X < -triangle.A.W && triangle.B.X < -triangle.B.W && triangle.C.X < -triangle.C.W)
            return 0;

        if (triangle.A.X > triangle.A.W && triangle.B.X > triangle.B.W && triangle.C.X > triangle.C.W)
            return 0;

        if (triangle.A.Y < -triangle.A.W && triangle.B.Y < -triangle.B.W && triangle.C.Y < -triangle.C.W)
            return 0;

        if (triangle.A.Y > triangle.A.W && triangle.B.Y > triangle.B.W && triangle.C.Y > triangle.C.W)
            return 0;

        if (triangle.A.Z < 0 && triangle.B.Z < 0 && triangle.C.Z < 0)
            return 0;

        if (triangle.A.Z > triangle.A.W && triangle.B.Z > triangle.B.W && triangle.C.Z > triangle.C.W)
            return 0;

        clippedTriangles[0] = triangle;
        return 1;
    }
}
