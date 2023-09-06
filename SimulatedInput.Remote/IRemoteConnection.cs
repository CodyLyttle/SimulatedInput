using System;
using SimulatedInput.Remote.Exceptions;

namespace SimulatedInput.Remote;

public interface IRemoteConnection : IDisposable
{
    RemoteConnectionState State { get; }
    
    event EventHandler<ConnectionClosedException>? ClosedByClient;
    event EventHandler<MessageDeserializerException>? FailedToDeserialize;
    event EventHandler<MessageFormatException>? BadMessageFormat;

    void BeginReceivingMessages();
    void CloseConnection();
}