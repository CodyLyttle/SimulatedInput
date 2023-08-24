using System.Collections.Generic;
using System.Linq;
using SimulatedInput.Library.Native;
using SimulatedInput.Library.Native.Structs;
using SimulatedInput.Library.Wrapper;

namespace SimulatedInput.Library;

public static class InputSender
{
    public static void Send(IInputAction input)
    {
        Send(new[] {input});
    }

    public static void Send(InputSequence sequence)
    {
        Send(sequence.ToArray());
    }

    public static void Send(IEnumerable<IInputAction> inputs)
    {
        INPUT[] inputArray = inputs.Select(x => x.ToInputStruct()).ToArray();
        NativeMethods.SendInput((uint)inputArray.Length, inputArray, INPUT.Size);
    }
}