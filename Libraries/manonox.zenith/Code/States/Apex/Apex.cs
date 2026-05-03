namespace Zenith.States;

[Title("Zenith Apex Default State")]
[Group("Zenith/States/Apex")]
[Icon("skateboarding")]
public partial class Apex : Controller.State {
    [RequireComponent] public FrameOfReferenceIntegrator FrameOfReferenceIntegrator { get; set; }
    public override Controller.Integrator Integrator => FrameOfReferenceIntegrator;


    [RequireComponent] public Look3d Look3d { get; set; }

    public override void Process() {
        if (Controller.IsOnGround) {
            CoyoteTimeTimer = 0f;
        }
    }

    public override void Move() {
        ThinkSprint();
        ThinkCrouch();
        ThinkLurch();

        TryJump(JumpHeight);

        if (Controller.IsOnGround) {
            Decelerate();
            Accelerate();
        } else {
            AirAccelerate();
        }
    }
}
