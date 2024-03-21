using System.Numerics;
namespace Benchmark.DepthTestBenchmarking;

public interface IRenderTarget2
{
    public int Width { get; }
    public int Height { get; }
    public void SetPixel(float x, float y, float z, in Vector4 color);
    public bool ZTest(float x, float y, float z);
}
