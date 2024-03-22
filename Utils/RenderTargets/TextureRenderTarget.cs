using System.Numerics;
using Utils.Components;
using Utils.Utils;
namespace Utils.RenderTargets;

public class TextureRenderTarget(Texture texture) : PresentableRenderTarget<Vector3>(texture.Width, texture.Height)
{
    public override void Present() => Array.Copy(ColorBuffer, texture.Data, ColorBuffer.Length);
    
    protected override Vector3 TransformColor(in Vector4 color) => color.AsVector3();
}
