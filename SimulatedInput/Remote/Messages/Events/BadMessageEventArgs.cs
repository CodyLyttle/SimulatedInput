using System;

namespace SimulatedInput.Remote.Messages.Events;

public class BadMessageEventArgs : EventArgs
{
    public RemoteMessageType MessageType { get; }
    public byte[] Data { get; }
    public string FailReason { get; }

    public BadMessageEventArgs(RemoteMessageType messageType, byte[] data, string failReason)
    {
        MessageType = messageType;
        Data = data;
        FailReason = failReason;
    }
}