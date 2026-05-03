namespace Zenith.States;


[Title("Zenith Noclip State")]
[Group("Zenith/States")]
[Icon("airplanemode_inactive")]
public class Noclip : Controller.State {
    [RequireComponent] public NoclipIntegrator NoclipIntegrator { get; set; }
    public override Controller.Integrator Integrator => NoclipIntegrator;

    [RequireComponent] public Look3d Look3d { get; set; }


    [Property]
    [Sync]
    public float SlowSpeed { get; set; } = 333f;

    [Property]
    [Sync]
    public float Speed { get; set; } = 1000f;

    [Property]
    [Sync]
    public float FastSpeed { get; set; } = 3000f;

    [Property]
    [Sync]
    public bool Smooth { get; set; } = true;

    [Property]
    [Group("Dynamics")]
    [ShowIf(nameof(Smooth), true)]
    [Sync]
    public float Acceleration { get; set; } = 10f;

    [Property]
    [Group("Dynamics")]
    [ShowIf(nameof(Smooth), true)]
    [Sync]
    public float Friction { get; set; } = 10f;


    [Property]
    public bool UseToggleAction { get; set; } = true;

    [Property]
    [InputAction]
    public string ToggleAction { get; set; } = "noclip";


    protected Controller.State PreviousState { get; set; } = null;

    public override void Move() {
        Controller.GroundObject = null;

        var speed = Speed;
        if (Input.Down("duck"))
            speed = SlowSpeed;
        if (Input.Down("run"))
            speed = FastSpeed;

        var wishDirection = Look3d.WishDirectionFree;
        if (Input.Down("jump")) {
            if (Look3d.WishDirectionUnrotated.x == 0f) {
                wishDirection += Look3d.HorizontalLook.Up;
            } else {
                wishDirection += (Look3d.HorizontalLook.Up + Look3d.Look.Up).Normal;
            }
        }
        wishDirection = wishDirection.ClampLength(1f);

        var targetVelocity = speed * wishDirection;
        if (!Smooth) {
            Controller.Velocity = targetVelocity;
        } else {
            Controller.Velocity = (Controller.Velocity - targetVelocity).WithFriction(Friction * Time.Delta, SlowSpeed) + targetVelocity;
            Controller.Velocity = Controller.Velocity.WithAcceleration(targetVelocity, Acceleration * Time.Delta);
        }

        Controller.Position += Controller.Velocity * Time.Delta;
    }

    protected override void OnFixedUpdate() {
        if (Input.Pressed(ToggleAction)) {
            if (!IsCurrent) {
                PreviousState = Controller.CurrentState;
                Controller.SetNextState(this);
            } else if (PreviousState.IsValid()) {
                Controller.SetNextState(PreviousState);
            } else {
                var notSelf = GetComponents<Controller.State>().Where(x => x != this).FirstOrDefault();
                if (notSelf.IsValid()) {
                    Controller.SetNextState(notSelf);
                }
            }
        }
    }
}