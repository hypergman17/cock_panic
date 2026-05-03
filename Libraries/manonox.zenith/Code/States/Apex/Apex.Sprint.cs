namespace Zenith.States;

public partial class Apex {
    public bool WantSprint => !IsProxy && Input.Down("run");
    public bool SprintActive { get; set; } = false;
    public bool IsSprinting => SprintProgress > 0f;
    public float SprintProgress => (!SprintActive || !Controller.IsOnGround) ? 0.0f : Controller.Velocity.Length.Remap(WalkSpeed, SprintSpeed);
    public TimeSince SprintInputBufferTimer = float.PositiveInfinity;
    public TimeUntil SprintIgnoreDirectionTimer = float.NegativeInfinity;

    public void ThinkSprint() {
        if (WantSprint) {
            SprintInputBufferTimer = 0f;
        }

        var wishdirectionNotPointingForward = Look3d.WishDirectionUnrotated.x < 0.7f;
        if (!SprintIgnoreDirectionTimer) {
            wishdirectionNotPointingForward = false;
        }

        if (Controller.IsOnGround && (wishdirectionNotPointingForward || Crouch.IsCrouched)) {
            SprintActive = false;
        } else if (SprintInputBufferTimer < 1f || SprintByDefault) {
            SprintActive = true;
            SprintInputBufferTimer = 0f;
        }

        if (SprintByDefault && WantSprint) {
            SprintActive = false;
        }
    }

    public void IgnoreSprintDirection(float seconds) {
        SprintIgnoreDirectionTimer = seconds;
    }
}
