using System.Runtime.InteropServices;

namespace SimulatedInput.Library.Native.Structs;

/// <summary>
/// Represents a Win32 HARDWAREINPUT structure. <br/>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-hardwareinput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct HARDWAREINPUT
{
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
}