namespace Zenith.Utility.Extensions;

public static class Vector3Extensions {
    public static void Deconstruct(this Vector3 v, out float x, out float y, out float z) {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public static (Vector3 OnPlane, Vector3 AlongNormal) SplitByNormal(this Vector3 v, in Vector3 normal) {
        var alongNormal = normal * v.Dot(normal);
        return (v - alongNormal, alongNormal);
    }
}
