namespace Zenith.States;


public partial class Apex {
    public Vector3 WishVelocity => CalculateGroundMaxSpeed() * Look3d.WishDirectionHorizontal;

    public float CalculateGroundMaxSpeed() {
        if (Crouch.IsCrouched) {
            return CrouchSpeed;
        }

        if (SprintActive) {
            return SprintSpeed;
        }

        return WalkSpeed;
    }

    public float CalculateGroundAcceleration() {
        var speed = Controller.Velocity.Length;
        var acceleration = Acceleration;
        if (!Crouch.IsCrouched) {
            if (speed < WalkSpeed - 1f) {
                acceleration = Acceleration;
                // Apex: walkSpeed of 200, lowSpeed = 140
                var lowSpeed = WalkSpeed * 0.7f; // Relative
                if (speed > lowSpeed) {
                    // Apex: acceleration = 450, lowAcceleration = 2500 (high)
                    var highAcceleration = Acceleration * 0.18f; // Relative
                    acceleration = highAcceleration;
                }
            } else {
                acceleration = (SprintSpeed - WalkSpeed) / SprintTime;
            }
        }

        return acceleration;
    }

    public void Decelerate(Vector3? wishDirection = null, float? deceleration = null, float? maxSpeed = null) {
        wishDirection ??= Look3d.WishDirectionHorizontal;
        deceleration ??= Deceleration;
        maxSpeed ??= CalculateGroundMaxSpeed();

        var speedAlongWishDirection = Controller.Velocity.Dot(wishDirection.Value);
        var allowedVelocity = speedAlongWishDirection.Clamp(0f, maxSpeed.Value) * wishDirection.Value;
        var velocityToDecelerate = Controller.Velocity - allowedVelocity;
        velocityToDecelerate = velocityToDecelerate.Normal * MathF.Max(velocityToDecelerate.Length - deceleration.Value * Time.Delta, 0f);
        Controller.Velocity = velocityToDecelerate + allowedVelocity;
    }

    public void Accelerate(Vector3? wishDirection = null, float? acceleration = null, float? maxSpeed = null) {
        wishDirection ??= Look3d.WishDirectionHorizontal;
        acceleration ??= CalculateGroundAcceleration();
        maxSpeed ??= CalculateGroundMaxSpeed();

        var speedAlongWishDirection = Controller.Velocity.Dot(wishDirection.Value);
        var maxAddedSpeed = maxSpeed.Value - speedAlongWishDirection;

        if (maxAddedSpeed <= 0f) {
            return;
        }

        var speedLimit = MathF.Max(Controller.Velocity.Length, maxSpeed.Value);
        Controller.Velocity += wishDirection.Value * Math.Min(acceleration.Value * Time.Delta, maxAddedSpeed);

        var newSpeed = Controller.Velocity.Length;
        if (newSpeed > speedLimit) {
            var factor = speedLimit / newSpeed;
            Controller.Velocity *= factor;
        }
    }

    public void AirAccelerate(Vector3? wishDirection = null, float? airAcceleration = null, float? maxSpeed = null) {
        wishDirection ??= Look3d.WishDirectionHorizontal;
        airAcceleration ??= AirAcceleration;
        maxSpeed ??= AirLimit;

        // TODO: Switch to "relative velocity", see https://apexmovement.tech/wiki/articles/Wiki%20help%3EHow%20the%20Apex%20engine%20handles%20Movement#Moving_Platforms

        var horizontalVelocity = Controller.Velocity.SubtractDirection(WorldTransform.Up);
        var currentSpeedAlongWishDirection = horizontalVelocity.Dot(wishDirection.Value);
        var addedMaxSpeed = maxSpeed.Value - currentSpeedAlongWishDirection;

        if (addedMaxSpeed <= 0f) {
            return;
        }

        Controller.Velocity += MathF.Min(airAcceleration.Value * Time.Delta, addedMaxSpeed) * wishDirection.Value;
    }
}
