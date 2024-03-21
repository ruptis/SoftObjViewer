using System.Numerics;
namespace Benchmark.DepthTestBenchmarking;

internal struct Viewport
{
    private int _width;
    private int _height;

    public Matrix4x4 Matrix;

    public void Set(int width, int height)
    {
        if (width == _width && height == _height) return;
        _width = width;
        _height = height;
        Matrix = Matrix4x4.CreateViewportLeftHanded(0, 0, width - 1, height - 1, 0, 1);
    }

    public bool IsOutOfViewport(in Vector4 position) =>
        position.X < 0 || position.X >= _width || position.Y < 0 || position.Y >= _height;
}
