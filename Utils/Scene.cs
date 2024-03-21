namespace Utils;

public record struct Scene(
    Camera Camera,
    IReadOnlyList<Light> Lights,
    Model Model);
