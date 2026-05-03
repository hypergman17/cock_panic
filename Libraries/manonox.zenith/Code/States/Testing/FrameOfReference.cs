namespace Zenith.States.Testing;


[Title("Zenith Frame Of Reference Test")]
[Group("Zenith/States")]
[Icon("fitness_center")]
public class FrameOfReference : Controller.State {
    [RequireComponent] public FrameOfReferenceIntegrator FrameOfReferenceIntegrator { get; set; }
    [RequireComponent] public Look3d Look3d { get; set; }
    public override Controller.Integrator Integrator => FrameOfReferenceIntegrator;

    public override void Move() {
        Controller.Velocity += Look3d.WishDirectionHorizontal * 2000f * Time.Delta;

        if (Input.Down("jump")) {
            Controller.Velocity = Controller.Velocity.SubtractDirection(WorldTransform.Up) + WorldTransform.Up * 200f;
            Controller.PreventGrounding();
        }

        if (Controller.IsOnGround) {
            Controller.Velocity = Controller.Velocity.WithFriction(0.1f);
        }
    }
}

