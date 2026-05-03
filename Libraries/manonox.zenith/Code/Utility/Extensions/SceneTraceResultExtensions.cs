namespace Zenith.Utility.Extensions;

public static class SceneTraceResultExtensions {
    public static Vector3 Travel(this SceneTraceResult trace) => trace.Fraction * (trace.EndPosition - trace.StartPosition);
    public static Vector3 Remainder(this SceneTraceResult trace) => (1f - trace.Fraction) * (trace.EndPosition - trace.StartPosition);
}
