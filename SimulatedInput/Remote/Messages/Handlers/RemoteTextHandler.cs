using System.Collections.Generic;
using SimulatedInput.Library;
using SimulatedInput.Library.Wrapper;

namespace SimulatedInput.Remote.Messages.Handlers;

public class RemoteTextHandler : IRemoteMessageHandler<RemoteText>
{
    public void Handle(RemoteText message)
    {
        IEnumerable<UnicodeInputAction> inputActions = KeyboardInputs.TextToKeyPresses(message.Content);
        InputSender.Send(inputActions);
    }
}