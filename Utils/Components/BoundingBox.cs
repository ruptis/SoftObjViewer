using System.Numerics;
namespace Utils.Components;

public class BoundingBox(Vector3 min, Vector3 max)
{
    public Vector3 Min { get; set; } = min;
    public Vector3 Max { get; set; } = max;

    public Vector3 Center
    {
        get => (Min + Max) / 2;
        set
        {
            Vector3 extents = Extents;
            Min = value - extents / 2;
            Max = value + extents / 2;
        }
    }
    public Vector3 Extents
    {
        get => Max - Min;
        set
        {
            Vector3 center = Center;
            Min = center - value / 2;
            Max = center + value / 2;
        }
    }

    public static BoundingBox FromTransform(Transform transform, Vector3 size)
    {
        Vector3 halfSize = size / 2;
        Vector3 min = transform.Position - halfSize;
        Vector3 max = transform.Position + halfSize;
        return new BoundingBox(min, max);
    }

    public bool Intersects(Ray ray, out float t)
    {
        Vector3 invDir = Vector3.One / ray.Direction;
        Vector3 tMin = (Min - ray.Origin) * invDir;
        Vector3 tMax = (Max - ray.Origin) * invDir;

        Vector3 t1 = Vector3.Min(tMin, tMax);
        Vector3 t2 = Vector3.Max(tMin, tMax);

        var tNear = MathF.Max(MathF.Max(t1.X, t1.Y), t1.Z);
        var tFar = MathF.Min(MathF.Min(t2.X, t2.Y), t2.Z);

        t = tNear;
        return tNear <= tFar;
    }

    public bool Intersects(Ray ray) => Intersects(ray, out _);
}
