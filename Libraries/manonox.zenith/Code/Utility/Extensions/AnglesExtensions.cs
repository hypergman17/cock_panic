namespace Zenith.Utility.Extensions;

public static class AnglesExtensions {
    public static void Deconstruct(this Angles v, out float pitch, out float yaw, out float roll) {
        pitch = v.pitch;
        yaw = v.yaw;
        roll = v.roll;
    }
}
