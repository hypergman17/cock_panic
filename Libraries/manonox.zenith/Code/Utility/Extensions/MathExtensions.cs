using System.Numerics;
using System.Runtime.CompilerServices;

namespace Zenith.Utility.Extensions;

public static class MathExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(this float x) => MathF.Abs(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acos(this float x) => MathF.Acos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acosh(this float x) => MathF.Acosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asin(this float x) => MathF.Asin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asinh(this float x) => MathF.Asinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan(this float x) => MathF.Atan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atanh(this float x) => MathF.Atanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BitDecrement(this float x) => MathF.BitDecrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BitIncrement(this float x) => MathF.BitIncrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cbrt(this float x) => MathF.Cbrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Ceiling(this float x) => MathF.Ceiling(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CopySign(this float x, float y) => MathF.CopySign(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(this float x) => MathF.Cos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cosh(this float x) => MathF.Cosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp(this float x) => MathF.Exp(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(this float x) => MathF.Floor(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FuseMultiplyAdd(this float x, float mul, float add) => MathF.FusedMultiplyAdd(x, mul, add);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float IEEERemainder(this float x, float y) => MathF.IEEERemainder(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ILogB(this float x) => MathF.ILogB(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(this float x) => MathF.Log(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(this float x, float y) => MathF.Log(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log10(this float x) => MathF.Log10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log2(this float x) => MathF.Log2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(this float x, float y) => MathF.Max(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MaxMagnitude(this float x, float y) => MathF.MaxMagnitude(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(this float x, float y) => MathF.Min(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MinMagnitude(this float x, float y) => MathF.MinMagnitude(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(this float x, float y) => MathF.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReciprocalEstimate(this float x) => MathF.ReciprocalEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReciprocalSqrtEstimate(this float x) => MathF.ReciprocalSqrtEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x) => MathF.Round(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x, int digits) => MathF.Round(x, digits);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x, MidpointRounding mode) => MathF.Round(x, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(this float x, int digits, MidpointRounding mode) => MathF.Round(x, digits, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ScaleB(this float x, int n) => MathF.ScaleB(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(this float x) => MathF.Sign(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(this float x) => MathF.Sin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float Sin, float Cos) SinCos(this float x) => MathF.SinCos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinh(this float x) => MathF.Sinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(this float x) => MathF.Sqrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tan(this float x) => MathF.Tan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tanh(this float x) => MathF.Tanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Truncate(this float x) => MathF.Truncate(x);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Abs(this double x) => Math.Abs(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acos(this double x) => Math.Acos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acosh(this double x) => Math.Acosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asin(this double x) => Math.Asin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asinh(this double x) => Math.Asinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan(this double x) => Math.Atan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atanh(this double x) => Math.Atanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double BitDecrement(this double x) => Math.BitDecrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double BitIncrement(this double x) => Math.BitIncrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cbrt(this double x) => Math.Cbrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Ceiling(this double x) => Math.Ceiling(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double CopySign(this double x, double y) => Math.CopySign(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cos(this double x) => Math.Cos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cosh(this double x) => Math.Cosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Exp(this double x) => Math.Exp(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Floor(this double x) => Math.Floor(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double FuseMultiplyAdd(this double x, double mul, double add) => Math.FusedMultiplyAdd(x, mul, add);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double IEEERemainder(this double x, double y) => Math.IEEERemainder(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ILogB(this double x) => Math.ILogB(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(this double x) => Math.Log(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(this double x, double y) => Math.Log(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log10(this double x) => Math.Log10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log2(this double x) => Math.Log2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(this double x, double y) => Math.Max(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double MaxMagnitude(this double x, double y) => Math.MaxMagnitude(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(this double x, double y) => Math.Min(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double MinMagnitude(this double x, double y) => Math.MinMagnitude(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Pow(this double x, double y) => Math.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReciprocalEstimate(this double x) => Math.ReciprocalEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReciprocalSqrtEstimate(this double x) => Math.ReciprocalSqrtEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double x) => Math.Round(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double x, int digits) => Math.Round(x, digits);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double x, MidpointRounding mode) => Math.Round(x, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(this double x, int digits, MidpointRounding mode) => Math.Round(x, digits, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ScaleB(this double x, int n) => Math.ScaleB(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(this double x) => Math.Sign(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sin(this double x) => Math.Sin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (double Sin, double Cos) SinCos(this double x) => Math.SinCos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sinh(this double x) => Math.Sinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sqrt(this double x) => Math.Sqrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tan(this double x) => Math.Tan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tanh(this double x) => Math.Tanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Truncate(this double x) => Math.Truncate(x);

    public static TSelf Add<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : IAdditionOperators<TSelf, TOther, TSelf> => x + rhs;
    public static TSelf Sub<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : ISubtractionOperators<TSelf, TOther, TSelf> => x - rhs;
    public static TSelf Mul<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : IMultiplyOperators<TSelf, TOther, TSelf> => x * rhs;
    public static TSelf Div<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : IDivisionOperators<TSelf, TOther, TSelf> => x / rhs;
    public static TSelf Mod<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : IModulusOperators<TSelf, TOther, TSelf> => x % rhs;
    public static TSelf RShift<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : IShiftOperators<TSelf, TOther, TSelf> => x << rhs;
    public static TSelf LShift<TSelf, TOther>(this TSelf x, in TOther rhs) where TSelf : IShiftOperators<TSelf, TOther, TSelf> => x >> rhs;
}