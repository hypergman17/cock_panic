namespace Zenith.States;

public partial class Apex {
    [Title("Apex Climb Zone Visualizer")]
    [Group("Zenith/States/Apex/Utility")]
    [Icon("tune")]
    public class ClimbZoneVisualizer : Controller.Module {
        [RequireComponent] public Apex Apex { get; set; }
        [RequireComponent] public Camera3d Camera3d { get; set; }
        [RequireComponent] public Climb Climb { get; set; }

        protected Vector3 LastBaselineVector { get; set; } = Vector3.Zero;
        protected Vector3 BaselineVector => Climb.ZoneBaseline;
        protected Vector3 InterpolatedBaselineVector => LastBaselineVector.LerpTo(BaselineVector, Scene.GetPhysicsInterpolationFraction());

        protected override void OnFixedUpdate() {
            LastBaselineVector = BaselineVector;
        }

        protected override void OnUpdate() {
            var trace = Game.SceneTrace
                .Ray(Camera3d.CameraWorldPosition, Camera3d.CameraWorldPosition + Camera3d.CameraWorldRotation.Forward * 1000f)
                .IgnoreGameObject(GameObject)
                .Run();

            if (!trace.Hit) { return; }

            var wallNormal = trace.Normal;
            if (MathF.Abs(wallNormal.Dot(WorldTransform.Up)) > 0.75f) { return; }

            var wallPosition = trace.HitPosition;
            var wallTangent = wallNormal.Cross(WorldTransform.Up).Normal;
            var wallUp = wallTangent.Cross(wallNormal);


            var baseline = (InterpolatedBaselineVector - wallPosition).Dot(WorldTransform.Up);
            baseline += Camera3d.BaseCameraHeight;
            var baselineOnWall = wallPosition + baseline * (WorldTransform.Up + wallUp.SubtractDirection(WorldTransform.Up));
            var baselineTransform = new Transform(baselineOnWall, Rotation.LookAt(wallNormal, WorldTransform.Up), 1f);
            var heightMul = 1f / MathF.Abs(wallUp.Dot(WorldTransform.Up));

            DebugOverlay.Box(new Vector3(0f, 0f, 0f), new Vector3(4f, 32f, 1f), Color.Cyan, 0f, baselineTransform);

            var miniHeight = Climb.ZoneMiniHeight * heightMul;
            var greenHeight = Climb.ZoneGreenHeight * heightMul;
            var neutralHeight = Climb.ZoneMaxHeight - Climb.ZoneGreenHeight - Climb.ZoneMiniHeight;

            DebugOverlay.Box(
                new Vector3(
                    0f,
                    0f,
                    miniHeight / 2f
                ),
                new Vector3(
                    4f,
                    32f,
                    miniHeight
                ),
                Color.Orange,
                0f,
                baselineTransform
            );

            DebugOverlay.Box(
                new Vector3(
                    0f,
                    0f,
                    greenHeight / 2f + miniHeight + 0.5f
                ),
                new Vector3(4f, 32f, greenHeight),
                Color.Green,
                0f,
                baselineTransform
            );

            DebugOverlay.Box(
                new Vector3(
                    0f,
                    -12f,
                    greenHeight / 2f + miniHeight + 0.5f
                ),
                new Vector3(3f, 8f, 1f),
                Color.Green,
                0f,
                baselineTransform
            );

            DebugOverlay.Box(
                new Vector3(
                    0f,
                    12f,
                    greenHeight / 2f + miniHeight + 0.5f
                ),
                new Vector3(3f, 8f, 1f),
                Color.Green,
                0f,
                baselineTransform
            );


            DebugOverlay.Box(
                new Vector3(
                    0f,
                    0f,
                    neutralHeight / 2f + greenHeight + miniHeight + 0.5f
                ),
                new Vector3(4f, 32f, neutralHeight),
                Color.White,
                0f,
                baselineTransform
            );
        }
    }
}
