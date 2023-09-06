using SimulatedInput.Core.Native;
using SimulatedInput.Core.Native.Structs;
using SimulatedInput.Core.Wrapper;

namespace SimulatedInput.Core;

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