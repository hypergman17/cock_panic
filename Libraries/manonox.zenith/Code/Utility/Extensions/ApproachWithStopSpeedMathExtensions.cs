namespace Zenith.Utility.Extensions;

public static class ApproachWithStopSpeedMathExtensions {
    public static float ApproachWithStopSpeed(this float self, in float target, in float stopSpeed, in float decay) {
        var delta = MathF.Max(MathF.Abs(self - target), stopSpeed) * decay * Time.Delta;
        return self.Approach(target, delta);
    }

    public static double Approach(this double self, double target, double delta) {
        if (self > target) {
            self -= delta;
            if (self < target) {
                return target;
            }
        } else {
            self += delta;
            if (self > target) {
                return target;
            }
        }

        return self;
    }


    public static double ApproachWithStopSpeed(this double self, in double target, in double stopSpeed, in double decay) {
        var delta = Math.Max(Math.Abs(self - target), stopSpeed) * decay * Time.Delta;
        return self.Approach(target, delta);
    }
}
