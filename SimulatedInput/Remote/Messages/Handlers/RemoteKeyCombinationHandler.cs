using System.Linq;
using SimulatedInput.Library;
using SimulatedInput.Library.Enum;

namespace SimulatedInput.Remote.Messages.Handlers;

public class RemoteKeyCombinationHandler : IRemoteMessageHandler<RemoteKeyCombination>
{
    public void Handle(RemoteKeyCombination message)
    {
        InputSequence sequence = new InputSequence();
        
        // Press keys down in the order they appear.
        foreach (VirtualKeyCode keyCode in message.KeyCodes)
        {
            sequence.Add(KeyboardInputs.KeyDown(keyCode));
        }
        
        // Release keys in opposite order they were pressed.
        foreach (VirtualKeyCode keyCode in message.KeyCodes.Reverse())
        {
            sequence.Add(KeyboardInputs.KeyUp(keyCode));
        }
        
        InputSender.Send(sequence);
    }
}