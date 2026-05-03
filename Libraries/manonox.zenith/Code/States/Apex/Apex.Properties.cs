namespace Zenith.States;

public partial class Apex {
    [Property]
    public float MaxSpeed { get; set; } = 5000f;


    // =================================

    [Property, Group("Ground"), Order(200)]
    public float Acceleration { get; set; } = 3000f;

    [Property, Group("Ground"), Order(200)]
    public float Deceleration { get; set; } = 1500f;

    [Property, Group("Ground"), Order(200)]
    public float WalkSpeed { get; set; } = 200f;

    // =================================

    [Property, Group("Sprint"), Order(300)]
    public float SprintSpeed { get; set; } = 300f;

    [Property, Group("Sprint"), Order(300)]
    public float SprintTime { get; set; } = 0.87f;

    [Property, Group("Sprint"), Order(300)]
    public bool SprintByDefault { get; set; } = true;

    // =================================

    [Property, Group("Crouch"), Order(400)]
    public float CrouchSpeed { get; set; } = 92f;

    [Property, Group("Crouch"), Order(400)]
    public float CrouchTime { get; set; } = 0.25f;

    [Property, Group("Crouch"), Order(400)]
    [Range(0f, 1f)]
    public float CrouchedAerialOffset { get; set; } = 0.17f;

    // =================================

    [Property, Group("Jump"), Order(500)]
    public float JumpHeight { get; set; } = 55f;

    [Property, Group("Jump"), Order(500)]
    public float MinJumpFatigueTime { get; set; } = 0.15f;

    [Property, Group("Jump"), Order(500)]
    public float MaxJumpFatigueTime { get; set; } = 0.75f;

    [Property, Group("Jump"), Order(500)]
    [Range(0f, 1f), Step(0.05f)]
    public float JumpFatigueHeightFrac { get; set; } = 0.3f;

    [Property, Group("Jump"), Order(500)]
    [Range(0f, 1f), Step(0.05f)]
    public float JumpUpSlopeBoost { get; set; } = 0.75f;

    [Property, Group("Jump"), Order(500)]
    public float JumpBufferTime { get; set; } = 0.2f;

    [Property, Group("Jump"), Order(500)]
    public float CoyoteTime { get; set; } = 0.2f;

    [Property, Group("Jump"), Order(500)]
    public bool AutoBhop { get; set; } = false;

    public enum ScrollDirection { Up, Down }
    [Property, Group("Jump"), Order(500)]
    public ScrollDirection? ScrollJump { get; set; } = ScrollDirection.Up;

    // =================================

    [Property, Group("Aerial"), Order(600)]
    public float AirAcceleration { get; set; } = 500f;

    [Property, Group("Aerial"), Order(600)]
    public float AirLimit { get; set; } = 70f;

    // =================================

    [Property, Group("Landing"), Order(600)]
    public float MinFallStunSpeed { get; set; } = 400f;

    [Property, Group("Landing"), Order(600)]
    public float MaxFallStunSpeed { get; set; } = 900f;

    [Property, Group("Landing"), Order(600)]
    [Range(0f, 1f), Step(0.05f)]
    public float FallStunStrength { get; set; } = 1f;
}
