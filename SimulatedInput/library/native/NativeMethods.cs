using System.Runtime.InteropServices;
using SimulatedInput.library.native.structs;

namespace SimulatedInput.library.native;

public class NativeMethods
{
    [DllImport("user32.dll")]
    public static extern uint SendInput(uint cInputs, INPUT [] pInputs, int cbSize);
}