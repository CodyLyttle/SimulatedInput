using System;

namespace SimulatedInput.Remote.Messages.Events;

public class UnknownMessageEventArgs : EventArgs
{
    public int TypeId { get; }
    public byte[] Content { get; }
    
    public UnknownMessageEventArgs(int typeId, byte[] content)
    {
        TypeId = typeId;
        Content = content;
    }
}