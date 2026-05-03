namespace Zenith.Utility;


[Title("Zenith Debug Display")]
[Group("Zenith/Utility")]
[Icon("bug_report")]
public class DebugDisplay : Component {
    [RequireComponent] public Controller Controller { get; set; }

    protected override void OnFixedUpdate() {
        var speed = Controller.Velocity.Length;
        var horizontalSpeed = Controller.Velocity.SubtractDirection(WorldTransform.Up).Length;
        OutputCenter(-4, $"Horizontal Speed: {horizontalSpeed:F1} / Speed: {speed:F1}");
        OutputCenter(-3, $"[{Controller.Velocity.x:F0} {Controller.Velocity.y:F0} {Controller.Velocity.z:F0}]");
        OutputCenter(-2, Controller.IsOnGround ? "Grounded" : "Airborne");
        OutputCenter(-1, Controller?.CurrentState?.ToString() ?? "No state");

        OutputCorner(0, $"Position: {WorldPosition.x:F2}, {WorldPosition.y:F2}, {WorldPosition.z:F2}");
        if (GetComponent<Look3d>() is var look3d && look3d.IsValid()) {
            OutputCorner(1, $"EyeAngles: {look3d.EyeAngles.pitch:F1}, {look3d.EyeAngles.yaw:F1}, {look3d.EyeAngles.roll:F1} ({look3d.FinalEyeAngles.pitch:F1}, {look3d.FinalEyeAngles.yaw:F1}, {look3d.FinalEyeAngles.roll:F1})");
        }
    }

    private void OutputCorner(int line, string text, float duration = 0f) {
        DebugOverlay.ScreenText(new(50f, Screen.Height * 0.8f + line * 18f), text, 14f, TextFlag.LeftBottom, Color.White, duration);
    }

    private void OutputCenter(int line, string text, float duration = 0f) {
        DebugOverlay.ScreenText(new(Screen.Width * 0.5f, Screen.Height * 0.7f + line * 18f), text, 14f, TextFlag.Center, Color.White, duration);
    }
}