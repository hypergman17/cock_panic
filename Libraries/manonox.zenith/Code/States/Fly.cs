namespace Zenith.States;


[Title("Zenith Fly State")]
[Group("Zenith/States")]
[Icon("flight")]
public class Fly : Controller.State {
    [RequireComponent] public PhysicalIntegrator PhysicalIntegrator { get; set; }
    [RequireComponent] public Look3d Look3d { get; set; }
    public override Controller.Integrator Integrator => PhysicalIntegrator;

    public override void Move() {
        Controller.Velocity += Look3d.WishDirectionFree * 2000f * Time.Delta;
        Controller.Velocity += Controller.Gravity * Time.Delta * 0.5f;
        Controller.Position += Controller.Velocity * Time.Delta;
        Controller.Velocity += Controller.Gravity * Time.Delta * 0.5f;
    }
}