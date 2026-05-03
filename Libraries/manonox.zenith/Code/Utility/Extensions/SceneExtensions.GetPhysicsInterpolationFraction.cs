namespace Zenith.Utility.Extensions;

public class GetPhysicsInterpolationFractionSystem : GameObjectSystem {
    public GetPhysicsInterpolationFractionSystem(Scene scene) : base(scene) {
        Listen(Stage.StartFixedUpdate, -100, FixedUpdate, "FixedUpdate");
    }

    void FixedUpdate() {
        SceneExtensions.TimeSinceLastFixedUpdate = 0f;
    }
}

public static partial class SceneExtensions {
    public static float GetPhysicsInterpolationFraction(this Scene scene) => TimeSinceLastFixedUpdate / scene.FixedDelta;
    public static TimeSince TimeSinceLastFixedUpdate { get; set; } = 0f;
}