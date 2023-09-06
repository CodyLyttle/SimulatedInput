using System.Runtime.InteropServices;
using SimulatedInput.Core.Native.Structs;

namespace SimulatedInput.Core.Native;

public class NativeMethods
{
    [DllImport("user32.dll")]
    public static extern uint SendInput(uint cInputs, INPUT [] pInputs, int cbSize);
}