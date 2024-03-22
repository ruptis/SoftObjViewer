using System.Numerics;
using System.Runtime.CompilerServices;
using GraphicsPipeline;
namespace Utils.RenderTargets;

public abstract class PresentableRenderTarget<T>(int width, int height) : IRenderTarget
{
    private readonly int[] _lockBuffer = new int[width * height];
    protected readonly T[] ColorBuffer = new T[width * height];
    protected readonly float[] DepthBuffer = new float[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;

    public abstract void Present();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract T TransformColor(in Vector4 color);

    public void SetPixel(float x, float y, in Vector4 color)
    {
        var index = GetIndex(x, y);
        ColorBuffer[index] = TransformColor(color);
    }

    public bool DepthTestAndLock(float x, float y, float z)
    {
        var index = GetIndex(x, y);
        var currentZ = DepthBuffer[index];
        
        while(z < currentZ)
        {
            if (Interlocked.CompareExchange(ref _lockBuffer[index], 1, 0) == 0)
            {
                if (Math.Abs(Interlocked.CompareExchange(ref DepthBuffer[index], z, currentZ) - currentZ) < float.Epsilon)
                    return true;
                
                Interlocked.Exchange(ref _lockBuffer[index], 0);
            }
            currentZ = DepthBuffer[index];
        }
        
        return false;
    }

    public void UnlockPixel(float x, float y) => 
        Interlocked.Exchange(ref _lockBuffer[GetIndex(x, y)], 0);

    public void Clear(in Vector4 color)
    {
        Array.Fill(ColorBuffer, TransformColor(color));
        Array.Fill(DepthBuffer, float.MaxValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(float x, float y) => 
        (int)(y * Width + x);

}
