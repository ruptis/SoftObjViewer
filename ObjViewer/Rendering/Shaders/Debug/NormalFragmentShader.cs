using System.Drawing;
using System.Numerics;
using GraphicsPipeline;
namespace ObjViewer.Rendering.Shaders;

public sealed class NormalFragmentShader : IFragmentShader<Vertex>
{
    public Texture? NormalTexture { get; set; }
    
    public void ProcessFragment(in Vector4 fragCoord, in Vertex input, out Color color)
    {
        Vector3 normal = NormalTexture?.SampleNormal(input.TextureCoordinates) ?? input.Normal;
        
        color = Color.FromArgb(
            (byte) (normal.X * 127 + 128),
            (byte) (normal.Y * 127 + 128),
            (byte) (normal.Z * 127 + 128)
        );
    }
}
