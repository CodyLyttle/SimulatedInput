using System;
using SimulatedInput.library.native.structs;
using SimulatedInput.library.native.unions;

namespace SimulatedInput.library.wrapper;

public sealed class VirtualKeyInputAction : IInputAction, IInteroperable<KEYBDINPUT>
{
    private static class Flag
    {
        public const uint KeyUp = 0x0002;
    }

    public InputType Type => InputType.Keyboard;
    public ushort KeyCode { get; }
    public bool IsKeyUp { get; }
    public uint Timestamp { get; set; } = 0;
    public UIntPtr ExtraInfo { get; set; } = UIntPtr.Zero;


    public VirtualKeyInputAction(ushort keyCode, bool isKeyUp)
    {
        if (keyCode > 254)
            throw new ArgumentOutOfRangeException(nameof(keyCode), keyCode,
                "Must be a virtual key code in the range of 0-254");

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
            wVK = KeyCode,
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
            type = (int)InputType.Keyboard,
            args = new InputArgumentsUnion(ToInteropStruct())
        };
    }
}