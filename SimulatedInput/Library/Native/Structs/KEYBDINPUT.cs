using System;
using System.Runtime.InteropServices;

namespace SimulatedInput.Library.Native.Structs;

/// <summary>
/// Represents a Win32 KEYBDINPUT structure. <br/>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-keybdinput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct KEYBDINPUT
{
    public ushort wVK;
    public ushort wScan;
    public uint dwFlags;
    public uint time;
    public UIntPtr dwExtraInfo;
}