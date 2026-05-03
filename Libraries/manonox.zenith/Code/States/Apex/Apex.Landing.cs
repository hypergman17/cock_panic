namespace Zenith.States;

using static Zenith.Controller;

public partial class Apex : Integrator.IEvents {
    public TimeSince LandTime { get; set; }
    public float LandSpeed { get; set; }

    void Integrator.IEvents.OnLand(GameObject gameObject, float landSpeed) {
        LandTime = 0f;
        LandSpeed = landSpeed;

        var fallStunAmount = landSpeed.Remap(MinFallStunSpeed, MaxFallStunSpeed);
        if (fallStunAmount > 0f) {
            // fallStunAmount = MathF.Pow( fallStunAmount, 2f );
            Controller.Velocity -= Controller.Velocity.SubtractDirection(WorldTransform.Up) * fallStunAmount * FallStunStrength;
        }

        LandTime = 0f;
        if (JumpFatigueActive) {
            JumpFatigueTimer = 0f;
            JumpFatigueActive = false;
        }
    }
}
