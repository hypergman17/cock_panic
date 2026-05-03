namespace Zenith.States;


public partial class Apex {
    public TimeSince CoyoteTimeTimer = float.PositiveInfinity;
    public TimeSince JumpFatigueTimer = float.PositiveInfinity;
    public TimeSince JumpInputBufferTimer = float.PositiveInfinity;
    public TimeSince JumpTimer = float.PositiveInfinity;
    public bool JumpFatigueActive { get; set; } = false;

    public bool WantNormalJump => !IsProxy && Input.Pressed("jump");
    public bool NormalJumpIsDown => !IsProxy && Input.Down("jump");
    public bool WantScrollJump => !IsProxy && ScrollJump.HasValue && (ScrollJump.Value switch {
        ScrollDirection.Up => Input.MouseWheel.y > 0,
        ScrollDirection.Down => Input.MouseWheel.y < 0,
        _ => throw new Exception("Wtf"),
    });

    public bool WantAnyJump => WantNormalJump || WantScrollJump;

    public bool TryJump(float jumpHeight) {
        if (WantNormalJump && JumpInputBufferTimer > JumpBufferTime) {
            JumpInputBufferTimer = 0f;
        }

        if (WantScrollJump) {
            JumpInputBufferTimer = JumpBufferTime - Time.Delta * 0.5f;
        }

        if (!Controller.IsOnGround && CoyoteTimeTimer > CoyoteTime) {
            return false;
        }

        if (JumpInputBufferTimer >= JumpBufferTime && !(AutoBhop && NormalJumpIsDown)) {
            return false;
        }

        JumpInputBufferTimer = float.PositiveInfinity;

        // Too frequent jumps
        if (JumpTimer < Scene.FixedDelta * 5.5f) {
            return false;
        }

        Jump(jumpHeight);
        return true;
    }

    public void Jump(float jumpHeight) {
        // Fatigue jumps are always "full-power", see https://apexmovement.tech/wiki/tech/General%20Tech%3EFundamentals%3EAir%3EFundamental%20Air%20Tech%20article#Coyote_time
        if (!Controller.IsOnGround) {
            JumpFatigueTimer = float.PositiveInfinity;
        }

        var height = jumpHeight * JumpFatigueHeightFrac.LerpTo(1f, ((float)JumpFatigueTimer).Remap(MinJumpFatigueTime, MaxJumpFatigueTime));
        var jumpSpeed = MathF.Sqrt(2f * Controller.Gravity.Length * height);

        if (Controller.IsOnGround) {
            var upDot = Controller.Velocity.SubtractDirection(Controller.GroundNormal).Dot(WorldTransform.Up);
            if (upDot >= 0f) {
                jumpSpeed += upDot * JumpUpSlopeBoost;
            }

            // if (FrameOfReferenceVelocity.HasValue) {
            //     var upDotGround = FrameOfReferenceVelocity.Value.Dot(WorldTransform.Up);
            //     if (upDotGround > 0f) {
            //         jumpSpeed += upDotGround * 0.1f;
            //     }
            // }
        }

        var jumpVelocity = WorldTransform.Up * jumpSpeed;

        JumpFatigueActive = true;
        Controller.Push(jumpVelocity);
        Post(x => x.OnJump());

        // if (Controller.GroundObject.IsValid()) {
        //     if (Controller.GroundObject.GetComponent<Rigidbody>() is var rigidbody && rigidbody is not null) {
        //         rigidbody.ApplyImpulseAt(WorldPosition, -jumpVelocity * rigidbody.Mass * 0.5f);
        //     }
        // }

        CoyoteTimeTimer = float.PositiveInfinity;

        ResetLurchTimer();
        JumpTimer = 0f;
        Controller.GroundObject = null;
        FrameOfReferenceIntegrator.FrameOfReference = null;

        // Slow down if the jump is a bhop, see https://apexmovement.tech/wiki/tech/General%20Tech%3EFundamentals%3EGround%3EFundamental%20Ground%20Movement%20article#Bunnyhop
        if (LandTime <= 0.1f) { // IsOnGround && ?
            (var horizontal, var vertical) = Controller.Velocity.SplitByNormal(WorldTransform.Up);
            if (horizontal.Length > SprintSpeed) {
                horizontal = horizontal.Approach(SprintSpeed, 100f);
            }

            Controller.Velocity = horizontal + vertical;
        }
    }
}
