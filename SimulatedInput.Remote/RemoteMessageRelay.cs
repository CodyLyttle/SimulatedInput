using System;
using System.Collections.Generic;
using SimulatedInput.Remote.Messages;
using SimulatedInput.Remote.Messages.Handlers;

namespace SimulatedInput.Remote;

public class RemoteMessageRelay : IRemoteMessageRelay
{
    private static readonly Dictionary<Type, object> MappedHandlers = new()
    {
        [typeof(RemoteKeyCombination)] = new RemoteKeyCombinationHandler(),
        [typeof(RemoteMouseButton)] = new RemoteMouseButtonHandler(),
        [typeof(RemoteMouseMove)] = new RemoteMouseMoveHandler(),
        [typeof(RemoteScroll)] = new RemoteScrollHandler(),
        [typeof(RemoteText)] = new RemoteTextHandler(),
    };

    public void HandleMessage<T>(T message)
    {
        if (MappedHandlers.TryGetValue(typeof(T), out object? handlerObj)
            && handlerObj is IRemoteMessageHandler<T> handler)
        {
            handler.Handle(message);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(message), typeof(T),
                "Unsupported message type");
        }
    }
}