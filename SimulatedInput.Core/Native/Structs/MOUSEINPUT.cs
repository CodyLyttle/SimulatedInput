using System.Runtime.InteropServices;

namespace SimulatedInput.Core.Native.Structs;

/// <summary>
/// Represents a Win32 MOUSEINPUT structure. <br/>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MOUSEINPUT
{
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}