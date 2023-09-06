using SimulatedInput.Core.Enum;
using SimulatedInput.Core.Native;
using SimulatedInput.Core.Native.Structs;

namespace SimulatedInput.Core.Wrapper;

public sealed class VirtualKeyInputAction : IInputAction, IInteroperable<KEYBDINPUT>
{
    private static class Flag
    {
        public const uint KeyUp = 0x0002;
    }

    public InputType Type => InputType.Keyboard;
    public VirtualKeyCode KeyCode { get; }
    public bool IsKeyUp { get; }
    public uint Timestamp { get; set; } = 0;
    public UIntPtr ExtraInfo { get; set; } = UIntPtr.Zero;


    public VirtualKeyInputAction(VirtualKeyCode keyCode, bool isKeyUp)
    {
        KeyCode = keyCode;
        IsKeyUp = isKeyUp;
    }

    public KEYBDINPUT ToInteropStruct()
    {
        uint flags = IsKeyUp 
            ? Flag.KeyUp 
            : 0;

        return new KEYBDINPUT
        {
            wVK = (ushort)KeyCode,
            wScan = 0,
            dwFlags = flags,
            time = Timestamp,
            dwExtraInfo = ExtraInfo
        };
    }
    
    public INPUT ToInputStruct()
    {
        return new INPUT
        {
            type = (uint)Type,
            args = new InputArgumentsUnion(ToInteropStruct())
        };
    }
}