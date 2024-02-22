using System.Numerics;
namespace ObjViewer.Rendering.Shaders.Phong;

public readonly record struct PhongShaderData(Vector3 LightDirection, Vector3 ViewDirection, Vector2 TextureCoordinates);
