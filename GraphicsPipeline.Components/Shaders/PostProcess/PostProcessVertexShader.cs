using System.Numerics;
namespace GraphicsPipeline.Components.Shaders.PostProcess;

public class PostProcessVertexShader : IVertexShader<PostProcessInput, Vector2>
{
    public void ProcessVertex(in PostProcessInput input, out Vector2 output, out Vector4 position)
    {
        position = new Vector4(input.Position, 0.0f, 1.0f);
        output = input.TextureCoordinates;
    }
}
