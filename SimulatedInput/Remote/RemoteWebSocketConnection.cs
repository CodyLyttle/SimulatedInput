using System;
using SimulatedInput.Remote.Exceptions;

namespace SimulatedInput.Remote;

// TODO: Implement RemoteWebSocketConnection.
public class RemoteWebSocketConnection : IRemoteConnection
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public RemoteConnectionState State { get; }
    public event EventHandler<ConnectionClosedException>? ClosedByClient;
    public event EventHandler<MessageDeserializerException>? FailedToDeserialize;
    public event EventHandler<MessageFormatException>? BadMessageFormat;
    public void BeginReceivingMessages()
    {
        throw new NotImplementedException();
    }

    public void CloseConnection()
    {
        throw new NotImplementedException();
    }
}