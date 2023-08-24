using System;
using SimulatedInput.library.native.structs;
using SimulatedInput.library.native.unions;

namespace SimulatedInput.library.wrapper;

public class UnicodeInputAction : IInputAction, IInteroperable<KEYBDINPUT>
{
    private static class Flag
    {
        public const uint KeyUp = 0x0002;
        public const uint Unicode = 0x0004; 
    }
    
    public InputType Type => InputType.Keyboard;
    public char Character { get; }
    public bool IsKeyUp { get; }
    public uint Timestamp { get; set; } = 0;
    public UIntPtr ExtraInfo { get; set; } = UIntPtr.Zero;
    

    public UnicodeInputAction(char character, bool isKeyUp)
    {
        Character = character;
        IsKeyUp = isKeyUp;
    }

    public KEYBDINPUT ToInteropStruct()
    {
        uint flags = IsKeyUp
            ? Flag.Unicode | Flag.KeyUp
            : Flag.Unicode;
        
        return new KEYBDINPUT
        {
            wVK = 0,
            wScan = Character,
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