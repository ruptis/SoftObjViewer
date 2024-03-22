namespace Utils.Components;

public record struct Scene(
    Camera Camera,
    IReadOnlyList<LightSource> Lights,
    Model Model);
