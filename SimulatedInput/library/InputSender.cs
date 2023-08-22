using System.ComponentModel;
using SimulatedInput.library.native;
using SimulatedInput.library.native.structs;
using SimulatedInput.library.wrapper;

namespace SimulatedInput.library;

public class InputSender
{
    public void SendKeystroke(IInputAction inputAction)
    {
        SendInputs(inputAction);
    }

    public void SendKeystrokes(params IInputAction[] inputActions)
    {
        SendInputs(inputActions);
    }

    private void SendInputs(params IInputAction[] inputActions)
    {
        if (inputActions.Length == 0)
            return;

        var inputs = new INPUT[inputActions.Length];
        for (var i = 0; i < inputActions.Length; i++)
        {
            inputs[i] = inputActions[i].ToInputStruct();
        }

        var size = (uint) inputActions.Length;
        uint executedInputs = NativeMethods.SendInput((uint) inputActions.Length, inputs, INPUT.Size);

        if (executedInputs < size)
        {
            uint remainingInputs = size - executedInputs;
            throw new Win32Exception($"Failed to execute {remainingInputs} inputs");
        }
    }
}