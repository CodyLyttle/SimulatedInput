using SimulatedInput.Core.Enum;
using SimulatedInput.Core.Wrapper;

namespace SimulatedInput.Core;

public static class KeyboardInputs
{
    public static VirtualKeyInputAction KeyDown(VirtualKeyCode virtualKeyCode)
    {
        return new VirtualKeyInputAction(virtualKeyCode, false);
    }

    public static ScanCodeInputAction KeyDown(ushort scanCode)
    {
        return new ScanCodeInputAction(scanCode, false);
    }

    public static UnicodeInputAction KeyDown(char character)
    {
        return new UnicodeInputAction(character, false);
    }

    public static VirtualKeyInputAction KeyUp(VirtualKeyCode virtualKeyCode)
    {
        return new VirtualKeyInputAction(virtualKeyCode, true);
    }

    public static ScanCodeInputAction KeyUp(ushort scanCode)
    {
        return new ScanCodeInputAction(scanCode, true);
    }

    public static UnicodeInputAction KeyUp(char character)
    {
        return new UnicodeInputAction(character, true);
    }

    public static IEnumerable<VirtualKeyInputAction> KeyPress(VirtualKeyCode virtualKeyCode)
    {
        yield return KeyDown(virtualKeyCode);
        yield return KeyUp(virtualKeyCode);
    }

    public static IEnumerable<ScanCodeInputAction> KeyPress(ushort scanCode)
    {
        yield return KeyDown(scanCode);
        yield return KeyUp(scanCode);
    }

    public static IEnumerable<UnicodeInputAction> KeyPress(char character)
    {
        yield return KeyDown(character);
        yield return KeyUp(character);
    }

    public static IEnumerable<UnicodeInputAction> TextToKeyPresses(string text)
    {
        foreach (char c in text)
        {
            yield return KeyDown(c);
            yield return KeyUp(c);
        }
    }
}