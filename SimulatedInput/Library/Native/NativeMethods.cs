using System.Runtime.InteropServices;
using SimulatedInput.Library.Native.Structs;

namespace SimulatedInput.Library.Native;

public class NativeMethods
{
    [DllImport("user32.dll")]
    public static extern uint SendInput(uint cInputs, INPUT [] pInputs, int cbSize);
}