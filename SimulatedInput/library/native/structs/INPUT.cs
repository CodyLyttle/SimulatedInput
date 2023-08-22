using System.Runtime.InteropServices;
using SimulatedInput.library.native.unions;

namespace SimulatedInput.library.native.structs;

[StructLayout(LayoutKind.Sequential)]
public struct INPUT
{
    public static readonly int Size = Marshal.SizeOf<INPUT>();
    
    public uint type;
    public InputArgumentsUnion args;
}