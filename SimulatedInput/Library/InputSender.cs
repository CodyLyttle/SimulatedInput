using System.ComponentModel;
using SimulatedInput.Library.Native;
using SimulatedInput.Library.Native.Structs;
using SimulatedInput.Library.Wrapper;

namespace SimulatedInput.Library;

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

    public void SendChar(char character)
    {
        SendInputs(new UnicodeInputAction(character, false),
            new UnicodeInputAction(character, true));
    }

    public void SendText(string text)
    {
        // TODO: Send in chunks to minimise memory footprint on large strings.
        int charCount = text.Length;
        var actions = new IInputAction[charCount * 2];
        for (var i = 0; i < charCount; i++)
        {
            int actionIndex = i * 2;
            actions[actionIndex] = new UnicodeInputAction(text[i], false);
            actions[++actionIndex] = new UnicodeInputAction(text[i], true);
        }
        
        SendInputs(actions);
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