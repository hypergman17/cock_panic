namespace Zenith;

public partial class Controller {
    public void Push(Vector3 v) {
        GroundObject = null;
        Velocity += v;
    }

    public void ApplyGravity(float multiplier = 1f) {
        Velocity += Gravity * Time.Delta * multiplier;
    }

    public void ApplyHalfGravity() => ApplyGravity(0.5f);
}