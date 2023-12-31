﻿using System.Runtime.InteropServices;
using SimulatedInput.Core.Native.Structs;

namespace SimulatedInput.Core.Native;

[StructLayout(LayoutKind.Explicit)]
public struct InputArgumentsUnion
{
    [FieldOffset(0)] 
    public  MOUSEINPUT Mouse;
    
    [FieldOffset(0)]
    public KEYBDINPUT Keyboard; 

    [FieldOffset(0)]
    public HARDWAREINPUT Hardware;

    public InputArgumentsUnion(MOUSEINPUT mouse) : this()
    {
        Mouse = mouse;
    }

    public InputArgumentsUnion(KEYBDINPUT keyboard): this()
    {
        Keyboard = keyboard;
    }

    public InputArgumentsUnion(HARDWAREINPUT hardware): this()
    {
        Hardware = hardware;
    }
}