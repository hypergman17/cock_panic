namespace Zenith.Integrators;

[Title("Zenith Noclip Integrator")]
[Icon("airplanemode_inactive")]
[Hide]
public class NoclipIntegrator : Controller.Integrator {
    public override bool ColliderEnabled => false;

    public override void PrePhysicsStep() {
        Controller.Body.MotionEnabled = false;
        Controller.Body.Velocity = Vector3.Zero;
        WorldPosition = Controller.Position;
    }

    public override void PostPhysicsStep() {

    }
}