using System.Numerics;
namespace Utils;

public class TextureRenderTarget(Texture texture) : PresentableRenderTarget<Vector3>(texture.Width, texture.Height)
{
    public override void Present() => Array.Copy(ColorBuffer, texture.Data, ColorBuffer.Length);
    
    protected override Vector3 TransformColor(in Vector4 color) => color.AsVector3();
}
