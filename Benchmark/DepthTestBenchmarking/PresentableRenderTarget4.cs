using System.Numerics;
namespace Benchmark.DepthTestBenchmarking;

public abstract class PresentableRenderTarget4<T>(int width, int height) : IRenderTarget2
    where T : unmanaged
{
    protected struct Pixel
    {
        public T R;
        public T G;
        public T B;
        public T A;
    }
    
    private readonly int[] _lockObjects = new int[width * height];
    protected readonly Pixel[] BackBuffer = new Pixel[width * height];
    protected readonly float[] DepthBuffer = new float[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;

    public abstract void Present();

    protected abstract Pixel TransformColor(in Vector4 color);

    public void SetPixel(float x, float y, float z, in Vector4 color)
    {
        var index = (int)(y * Width + x);
        BackBuffer[index] = TransformColor(color);

        Interlocked.Exchange(ref _lockObjects[index], 0);
    }

    public bool ZTest(float x, float y, float z)
    {
        var index = (int)(y * Width + x);
        var currentZ = DepthBuffer[index];
        
        while(z < currentZ)
        {
            if (Interlocked.CompareExchange(ref _lockObjects[index], 1, 0) == 0)
            {
                if (Math.Abs(Interlocked.CompareExchange(ref DepthBuffer[index], z, currentZ) - currentZ) < float.Epsilon)
                    return true;
                
                Interlocked.Exchange(ref _lockObjects[index], 0);
            }
            currentZ = DepthBuffer[index];
        }
        
        return false;
    }

    public void Clear(in Vector4 color)
    {
        Array.Fill(BackBuffer, TransformColor(color));
        Array.Fill(DepthBuffer, float.MaxValue);
    }
}
