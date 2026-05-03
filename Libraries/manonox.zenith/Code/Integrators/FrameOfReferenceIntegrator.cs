namespace Zenith.Integrators;

using static Controller;


[Title("Zenith Frame of Reference Integrator")]
[Icon("anchor")]
[Hide]
public class FrameOfReferenceIntegrator : Integrator {
    [Property]
    public TagSet GroundExcludeTags { get; set; } = [];

    [Property]
    public TagSet LinkExcludeTags { get; set; } = [];

    [Property]
    [Range(0f, 500f, false)]
    public float RampSlideSpeed { get; set; } = 100f;

    [Property]
    public bool MoveWithStaticBodies { get; set; } = false;

    public bool BypassPhysics { get; set; } = false;
    public bool BypassLinkedMovement { get; set; } = false;

    [Property, Feature("Debug"), ReadOnly]
    public GameObject FrameOfReference { get; set; } = null;
    public Vector3 FrameOfReferenceLocalPosition { get; set; } = Vector3.Zero;
    public Vector3 FrameOfReferenceWorldPosition => FrameOfReference.WorldTransform.PointToWorld(FrameOfReferenceLocalPosition);

    [Property, Feature("Debug"), ReadOnly]
    public Vector3 FrameOfReferenceVelocity => CalculateFrameOfReferenceVelocity(FrameOfReference, FrameOfReferenceLocalPosition);

    protected GameObject PreviousFrameOfReference { get; set; } = null;
    protected Vector3 PreviousFrameOfReferenceLocalPosition { get; set; } = Vector3.Zero;

    protected (GameObject, TimeSince)? FrameOfReferenceBuffer { get; private set; } = null;
    protected Vector3 RecordedFrameOfReferenceWorldPosition { get; set; } = Vector3.Zero;

    public bool WasOnGround { get; protected set; }

    public override void PreMove() {
        if (!FrameOfReference.IsValid()) { FrameOfReference = null; }
        if (!PreviousFrameOfReference.IsValid()) { PreviousFrameOfReference = null; }

        if (FrameOfReference.IsValid()) {
            MoveWithFrameOfReference();
        }

        PreviousFrameOfReference = FrameOfReference;
        PreviousFrameOfReferenceLocalPosition = FrameOfReferenceLocalPosition;

        WasOnGround = Controller.IsOnGround;

        BypassLinkedMovement = false;
    }

    public override void PostMove() {
        Integrate();

        if (FrameOfReference.IsValid()) {
            FrameOfReferenceBuffer = (FrameOfReference, 0f);
        }

        if (FrameOfReference != PreviousFrameOfReference) {
            var previousFrameOfReferenceVelocity = CalculateFrameOfReferenceVelocity(PreviousFrameOfReference, PreviousFrameOfReferenceLocalPosition);

            if (!PreviousFrameOfReference.IsValid() || !PreviousFrameOfReference.Tags.HasAny(LinkExcludeTags)) {
                Controller.Velocity += previousFrameOfReferenceVelocity;
            }

            if (!FrameOfReference.IsValid() || !FrameOfReference.Tags.HasAny(LinkExcludeTags)) {
                Controller.Velocity -= FrameOfReferenceVelocity;
            }
        }
    }

    public Vector3 PreIntegrateVelocity { get; protected set; }
    protected void Integrate() {
        PreIntegrateVelocity = Controller.Velocity;

        if (Controller.IsOnGround) {
            Controller.Velocity = Controller.Velocity.SubtractDirection(WorldTransform.Up);
        } else {
            Controller.ApplyGravity(0.5f);
        }

        var motion = Controller.Velocity * Time.Delta;
        MoveAndSlideResult result;
        if (Controller.IsOnGround) {
            result = Controller.MoveAndSlideWithStep(Controller.Position, motion);
        } else {
            result = Controller.MoveAndSlide(Controller.Position, motion, new() { FlatWalls = false });
        }

        Controller.Position = result.EndPosition;
        ResolveCollisions(result.Collisions);


        var frameOfReferenceVerticalSpeed = FrameOfReferenceVelocity.Dot(WorldTransform.Up);
        var extraGroundCheckDistance = frameOfReferenceVerticalSpeed.Abs() * Time.Delta;

        var ground = Controller.MoveAndSlide(
            Controller.Position,
            WorldTransform.Down * (Controller.Margin * 4f + extraGroundCheckDistance),
            new() { FlatWalls = false, StopAtGround = true }
        );

        if (ground.Hit) {
            if (ground.IsStuck) {
                if (Controller.Velocity.Dot(WorldTransform.Down) > 0f) {
                    Controller.Velocity = Controller.Velocity.SubtractDirection(WorldTransform.Down);
                }
            }
        }

        ground.Collisions = [.. ground.Collisions.Enumerate().Select( x => {
            var originalNormal = ground.Normals[x.Index];
            var realNormal = Controller.GetRealNormal( x.Item );
            if (originalNormal.Dot(WorldTransform.Up) > realNormal.Dot(WorldTransform.Up) ) {
                x.Item.Normal = originalNormal;
            } else {
                x.Item.Normal = realNormal;
            }

            return x.Item;
        } )];

        var validGrounds = ground.Collisions
            .Where(x => Controller.IsValidGround(x))
            .Where(x => !GroundExcludeTags.HasAny(x.Tags));
        if (validGrounds.Any() && Controller.Velocity.Dot(WorldTransform.Up) < RampSlideSpeed) {
            var validGround = validGrounds.First();
            var newGroundObject = validGround.GameObject;
            var newGroundNormal = validGround.Normal;

            if (Controller.GroundObject != newGroundObject) {
                Controller.GroundObject = newGroundObject;
                FrameOfReference = Controller.GroundObject;

                if (!WasOnGround && Controller.IsOnGround) {
                    var landSpeed = PreIntegrateVelocity.Dot(-newGroundNormal);
                    Post(x => x.OnLand(newGroundObject, landSpeed));
                }
            }

            if (Controller.IsOnGround) {
                Controller.GroundNormal = newGroundNormal;
                FrameOfReferenceLocalPosition = Controller.GroundObject.WorldTransform.PointToLocal(
                    validGround.HitPosition
                );

                var extraSafeOffset = frameOfReferenceVerticalSpeed.Max(-100f).Abs() * Time.Delta * 0.1f;
                var offset = WorldTransform.Up * (Controller.Margin - 0.01f + extraSafeOffset);
                var target = Controller.Trace(ground.EndPosition, ground.EndPosition + offset).EndPosition;
                var move = target - Controller.Position;
                WorldPosition += move;
                Controller.Position += move;
            }

            if (validGround.Collider?.Rigidbody is var rigidbody && rigidbody.IsValid()) {
                var massCenterLocal = rigidbody.OverrideMassCenter ? rigidbody.MassCenterOverride : rigidbody.MassCenter;
                var massCenterWorld = rigidbody.WorldTransform.PointToWorld(massCenterLocal);
                var pushPoint = validGround.HitPosition.LerpTo(massCenterWorld, 0.25f);
                if (!WasOnGround) {
                    var solution = SolveElasticCollision(
                        Vector3.Up,
                        PreIntegrateVelocity - PreIntegrateVelocity.SubtractDirection(WorldTransform.Up),
                        rigidbody.Velocity,
                        Controller.Mass,
                        rigidbody.Mass
                    );

                    if (solution is (var _, var impulse2)) {
                        // If rigidbody is pushed on same frame, FrameOfReferenceVelocity gets fucked up
                        Task.FixedUpdate().ContinueWith(async x => {
                            if (!Task.IsValid) { return; }
                            await Task.FixedUpdate();
                            if (!rigidbody.IsValid()) { return; }
                            rigidbody.ApplyImpulseAt(pushPoint, impulse2 * rigidbody.Mass);
                        });
                    }
                }

                // Apply Force to ground
                // but(!!!) prevent double gravity
                if (rigidbody.GetVelocity().Dot(Controller.Gravity.Normal) < 20f) {
                    rigidbody.ApplyForceAt(pushPoint, (Controller.Gravity * Controller.Mass).ProjectOnNormal(Controller.GroundNormal));
                }
            }
        } else {
            Controller.GroundObject = null;
        }

        if (WasOnGround && !Controller.IsOnGround) {
            FrameOfReference = null;
        }

        if (Controller.IsOnGround) {
            Controller.Velocity = Controller.Velocity.SubtractDirection(WorldTransform.Up);
        } else {
            Controller.ApplyGravity(0.5f);
        }
    }

    protected void ResolveCollisions(IEnumerable<SceneTraceResult> collisions) {
        foreach (var collision_ in collisions) {
            var collision = collision_;
            var realNormal = collision.Normal.Normal;
            if (Controller.IsOnGround && collision.Normal.Dot(WorldTransform.Up) > 0.0001f) {
                collision.Normal = Controller.GetRealNormal(collision);
            }

            if (Controller.IsValidGround(collision)) {
                if (Controller.Velocity.Dot(WorldTransform.Up) < 0f) {
                    Controller.Velocity = Controller.Velocity.SubtractDirection(WorldTransform.Up);
                }

                continue;
            }

            if (Controller.IsOnGround) {
                collision.Normal = collision.Normal.SubtractDirection(WorldTransform.Up).Normal;
                if (collision.Normal.IsNearZeroLength) {
                    collision.Normal = WorldTransform.Down;
                }
            }

            if (collision.Normal.Dot(Controller.Velocity) >= 0f) {
                continue;
            }

            if (collision.StartedSolid && collision.GameObject != Controller.GroundObject) {
                continue;
            }

            if (collision.Body.BodyType != PhysicsBodyType.Dynamic) {
                Controller.Velocity = Controller.Velocity.SubtractDirection(collision.Normal);
            }
        }
    }

    protected static (Vector3 impulse1, Vector3 impulse2)? SolveElasticCollision(Vector3 normal, Vector3 velocity1, Vector3 velocity2, float mass1, float mass2) {
        var relativeVelocity = velocity1 - velocity2;
        var relativeVelocityAlongNormal = relativeVelocity.Dot(normal);
        if (relativeVelocityAlongNormal >= 0f) {
            return null;
        }

        mass1 = Math.Max(mass1, 1f);
        mass2 = Math.Max(mass2, 1f);

        const float BOUNCYNESS = 0.1f;
        var j = -(1f + BOUNCYNESS) * relativeVelocityAlongNormal;
        j /= 1f / mass1 + 1f / mass2;
        return (j / mass1 * normal, -j / mass2 * normal);
    }

    // Physics

    private static Vector3 CalculateFrameOfReferenceVelocity(GameObject gameObject, in Vector3 localPosition) {
        var velocity = gameObject?.GetComponent<Collider>()
            ?.GetVelocityAtPoint(
                gameObject.WorldTransform.PointToWorld(localPosition)
            ) ?? Vector3.Zero;
        if (velocity.IsNaN) { return Vector3.Zero; }
        if (velocity.IsInfinity) { return Vector3.Zero; }
        velocity = velocity.ClampLength(100000f);
        return velocity;
    }

    private static readonly string noCollidePairSelf = "FrameOfReferenceIntegrator_self";
    private static readonly string noCollidePairOther = "FrameOfReferenceIntegrator_other";
    private static readonly Sandbox.Physics.CollisionRules.Pair noCollidePair = new(noCollidePairSelf, noCollidePairOther);

    protected Vector3 BodyVelocityBeforeStep { get; set; }
    protected Vector3 CurrentBodyVelocity { get; set; }
    public override void PrePhysicsStep() {
        ProjectSettings.Collision.Pairs.Add(noCollidePair, Sandbox.Physics.CollisionRules.Result.Ignore);
        Controller.Body.Tags.Add(noCollidePairSelf);
        if (FrameOfReference.IsValid()) {
            FrameOfReference.Tags.Add(noCollidePairOther);
        }

        if (FrameOfReference.IsValid()) {
            RecordedFrameOfReferenceWorldPosition = FrameOfReferenceWorldPosition;
        }

        var positionalVelocity = (Controller.Position - WorldPosition) / Time.Delta;
        BodyVelocityBeforeStep = positionalVelocity.LerpTo(Controller.Velocity, 0.5f, false);
        Controller.Body.Velocity = BodyVelocityBeforeStep;
        CurrentBodyVelocity = Controller.Body.Velocity;
        Controller.Collider.Friction = 0f;
        Controller.Collider.Elasticity = 0f;
        Controller.Collider.RollingResistance = 10000f;
        Controller.Body.MotionEnabled = !BypassPhysics;
    }

    public override void PostPhysicsStep() {
        if (BypassPhysics) {
            WorldPosition = Controller.Position;
        } else {
            Controller.Velocity += Controller.Body.Velocity - BodyVelocityBeforeStep;
        }

        BypassPhysics = false;

        ProjectSettings.Collision.Pairs.Remove(noCollidePair);
        Controller.Body.Tags.Remove(noCollidePairSelf);

        if (FrameOfReference.IsValid()) {
            FrameOfReference.Tags.Remove(noCollidePairOther);
            MoveWithFrameOfReference();
        }
    }

    public override void OnCollisionStart(Collision collision)
        => HandleCollision(collision);

    public override void OnCollisionUpdate(Collision collision)
        => HandleCollision(collision);

    public void HandleCollision(Collision collision) {
        if (FrameOfReferenceBuffer is (GameObject lastFrameOfReference, TimeSince time)) {
            if (time > Scene.FixedDelta * 2.1f) { return; }
            if (collision.Other.GameObject != lastFrameOfReference) { return; }

            // Restore original velocity
            collision.Self.Body.Velocity = CurrentBodyVelocity;
        }

        CurrentBodyVelocity = collision.Self.Body.Velocity;
    }

    protected override void OnFixedUpdate() {
        if (Controller.CurrentIntegrator != this) {
            FrameOfReference = null;
        }
    }

    private void MoveWithFrameOfReference() {
        if (BypassLinkedMovement) { return; }
        if (!FrameOfReference.IsValid()) { FrameOfReference = null; return; }
        if (FrameOfReference.Tags.HasAny(LinkExcludeTags)) { return; }
        var collider = FrameOfReference.GetComponentInChildren<Collider>();
        if (!MoveWithStaticBodies && (collider?.Static ?? false)) { return; }
        var frameOfReferenceMoved = FrameOfReferenceWorldPosition - RecordedFrameOfReferenceWorldPosition;
        RecordedFrameOfReferenceWorldPosition = FrameOfReferenceWorldPosition;
        Controller.Position += frameOfReferenceMoved;
        WorldPosition += frameOfReferenceMoved;
    }
}