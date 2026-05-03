namespace Zenith.Integrators;


[Title("Zenith Physical Integrator")]
[Icon("fitness_center")]
[Hide]
public class PhysicalIntegrator : Controller.Integrator {
    public override void PrePhysicsStep() {
        Controller.Body.Velocity = (Controller.Position - WorldPosition) / Time.Delta;
        Controller.Body.Sleeping = false;
    }

    public override void PostPhysicsStep() {
        Controller.Velocity = Controller.Body.Velocity;
    }

    public override void OnCollisionStart(Collision collision) {

    }

    public override void OnCollisionUpdate(Collision collision) {

    }
}