namespace Zenith.Utility;

public static class Easing {
    public static float Smoothstep(in float x) {
        if (x <= 0f) { return 0f; }
        if (x >= 1f) { return 1f; }
        return x * x * (3f - 2f * x);
    }

    public static float BackIn(in float x) {
        const float C1 = 1.70158f;
        const float C3 = C1 + 1f;

        return C3 * x * x * x - C1 * x * x;
    }

    public static float BackOut(in float x) {
        const float C1 = 1.70158f;
        const float C3 = C1 + 1f;

        return 1f + C3 * MathF.Pow(x - 1f, 3f) + C1 * MathF.Pow(x - 1f, 2f);
    }

    public static float BackInOut(in float x) {
        const float C1 = 1.70158f;
        const float C2 = C1 * 1.525f;

        return x < 0.5f
            ? MathF.Pow(2f * x, 2f) * ((C2 + 1f) * 2f * x - C2) / 2f
            : (MathF.Pow(2f * x - 2f, 2f) * ((C2 + 1f) * (x * 2f - 2f) + C2) + 2f) / 2f;
    }

    public static float CircIn(in float x) {
        return 1f - MathF.Sqrt(1f - MathF.Pow(x, 2f));
    }

    public static float CircOut(in float x) {
        return MathF.Sqrt(1f - MathF.Pow(x - 1f, 2f));
    }

    public static float CircInOut(in float x) {
        return x < 0.5f
            ? (1f - MathF.Sqrt(1f - MathF.Pow(2f * x, 2f))) / 2f
            : (MathF.Sqrt(1f - MathF.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
    }

    public static float ElasticIn(in float x) {
        const float C4 = 2f * MathF.PI / 3f;

        return x <= 0f
            ? 0f
            : x >= 1f
            ? 1f
            : -MathF.Pow(2f, 10f * x - 10f) * MathF.Sin((x * 10f - 10.75f) * C4);
    }

    public static float ElasticOut(in float x) {
        const float C4 = 2f * MathF.PI / 3f;

        return x <= 0f
            ? 0f
            : x >= 1f
            ? 1f
            : MathF.Pow(2f, -10f * x) * MathF.Sin((x * 10f - 0.75f) * C4) + 1f;
    }

    public static float ElasticInOut(in float x) {
        const float C5 = 2f * MathF.PI / 4.5f;

        return x <= 0f
            ? 0f
            : x >= 1f
            ? 1f
            : x < 0.5f
            ? -(MathF.Pow(2f, 20f * x - 10f) * MathF.Sin((20f * x - 11.125f) * C5)) / 2f
            : MathF.Pow(2f, -20f * x + 10f) * MathF.Sin((20f * x - 11.125f) * C5) / 2f + 1f;
    }
}