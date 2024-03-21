using System.Numerics;
namespace Benchmark.DepthTestBenchmarking;

public abstract class PresentableRenderTarget3<T> : IRenderTarget2 where T : unmanaged
{
    protected struct Pixel
    {
        public T R;
        public T G;
        public T B;
        public T A;
    }
    private readonly ReaderWriterLockSlim[] _lockObjects;

    protected readonly Pixel[] BackBuffer;
    protected readonly float[] DepthBuffer;

    protected PresentableRenderTarget3(int width, int height)
    {
        _lockObjects = new ReaderWriterLockSlim[width * height];
        BackBuffer = new Pixel[width * height];
        DepthBuffer = new float[width * height];
        Width = width;
        Height = height;

        for (var i = 0; i < _lockObjects.Length; i++)
            _lockObjects[i] = new ReaderWriterLockSlim();
    }

    public int Width { get; }
    public int Height { get; }

    public abstract void Present();

    protected abstract Pixel TransformColor(in Vector4 color);

    public void SetPixel(float x, float y, float z, in Vector4 color)
    {
        var index = (int)(y * Width + x);
        BackBuffer[index] = TransformColor(color);

        _lockObjects[index].ExitWriteLock();
    }

    public bool ZTest(float x, float y, float z)
    {
        var index = (int)(y * Width + x);
        
        _lockObjects[index].EnterWriteLock();

        if (z < DepthBuffer[index])
        {
            DepthBuffer[index] = z;
            return true;
        }
        
        _lockObjects[index].ExitWriteLock();
        
        return false;
    }

    public void Clear(in Vector4 color)
    {
        Array.Fill(BackBuffer, TransformColor(color));
        Array.Fill(DepthBuffer, float.MaxValue);
    }

    private bool ZTestInterloked(int index, float z, float currentZ) =>
        Math.Abs(Interlocked.CompareExchange(ref DepthBuffer[index], z, currentZ) - currentZ) < float.Epsilon;
}
