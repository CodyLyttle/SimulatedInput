using System.Runtime.InteropServices;

namespace SimulatedInput.Core.Native.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct INPUT
{
    public static readonly int Size = Marshal.SizeOf<INPUT>();
    
    public uint type;
    public InputArgumentsUnion args;
}