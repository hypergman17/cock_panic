using System.IO;
using System.Runtime.CompilerServices;

namespace Zenith.Utility.Extensions;

public static class DebugOverlaySystemExtensions {

    private static Dictionary<int, (string key, RealTimeUntil time)> slots = [];

    private static int FindSlotFor(string key) {
        var slot = -1;
        foreach (var pair in slots) {
            if (pair.Value.key == key) {
                slot = pair.Key;
            }
        }

        if (slot < 0) {
            slot++;
            while (slots.ContainsKey(slot)) {
                var entry = slots[slot];
                if (entry.time) {
                    break;
                }

                slot++;

                if (slot > 64) {
                    slot = -1;
                    break;
                }
            }
        }

        if (slot < 0) {
            return -1;
        }

        return slot;
    }

    public static void DebugText(this DebugOverlaySystem dbg,
        string text,
        Color color = default,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0
    ) {
        var key = $"{Path.GetFileName(file)}_{member}({line}): {text}";
        var slot = FindSlotFor(key);
        dbg.ScreenText(new(32, 32 + slot * 16), key, 14, TextFlag.Left, color);
    }

    public static void Variable(
        this DebugOverlaySystem dbg,
        string name,
        object value,
        Color color = default,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0
    ) {
        var key = $"{Path.GetFileName(file)}_{member}({line}): {name}";
        var slot = FindSlotFor(key);
        slots[slot] = (key, (RealTimeUntil)1f);
        dbg.ScreenText(new(32, 32 + slot * 16), $"{key} = {value}", 14, TextFlag.Left, color);
    }

    public static void Basis(this DebugOverlaySystem dbg, in Vector3 position, in Rotation rotation, float size, bool overlay = false) {
        dbg.Line(position, position + rotation.Forward * size, Color.Red, overlay: overlay);
        dbg.Line(position, position + rotation.Right * size, Color.Green, overlay: overlay);
        dbg.Line(position, position + rotation.Up * size, Color.Blue, overlay: overlay);
    }
}
