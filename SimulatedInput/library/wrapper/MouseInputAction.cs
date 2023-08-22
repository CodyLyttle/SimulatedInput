using System;
using SimulatedInput.library.native.structs;
using SimulatedInput.library.native.unions;

namespace SimulatedInput.library.wrapper;

public class MouseInputAction : IInputAction, IInteroperable<MOUSEINPUT>
{
    public static class Flag
    {
        public const uint Move = 0x0001;
        public const uint LeftDown = 0x0002;
        public const uint LeftUp = 0x0004;
        public const uint RightDown = 0x0008;
        public const uint RightUp = 0x0010;
        public const uint MiddleDown = 0x0020;
        public const uint MiddleUp = 0x0040;
        public const uint XDown = 0x0080;
        public const uint XUp = 0x0100;
        public const uint Wheel = 0x0800;
        public const uint HWheel = 0x1000;
        public const uint MoveNoCoalesce = 0x2000;
        public const uint VirtualDesk = 0x4000;
        public const uint Absolute = 0x8000;
    }
    
    public InputType Type => InputType.Mouse;
    public int X { get; set; }
    public int Y { get; set; }
    public uint MouseData { get; set; }
    public uint Flags { get; set; }
    public uint Time { get; set; }
    public IntPtr ExtraInfo { get; set; }

    public MOUSEINPUT ToInteropStruct()
    {
        return new MOUSEINPUT
        {
            dx = X,
            dy = Y,
            mouseData = MouseData,
            dwFlags = Flags,
            time = Time,
            dwExtraInfo = ExtraInfo
        };
    }
    
    public INPUT ToInputStruct()
    {
        return new INPUT
        {
            type = (int)InputType.Mouse,
            args = new InputArgumentsUnion(ToInteropStruct())
        };
    }
}