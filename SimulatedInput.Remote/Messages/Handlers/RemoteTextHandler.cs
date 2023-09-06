using SimulatedInput.Core;
using SimulatedInput.Core.Wrapper;

namespace SimulatedInput.Remote.Messages.Handlers;

public class RemoteTextHandler : IRemoteMessageHandler<RemoteText>
{
    public void Handle(RemoteText message)
    {
        IEnumerable<UnicodeInputAction> inputActions = KeyboardInputs.TextToKeyPresses(message.Content);
        InputSender.Send(inputActions);
    }
}