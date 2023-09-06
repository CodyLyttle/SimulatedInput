namespace SimulatedInput.Remote.Tests.Stubs;

public class MessageRelayStub : IRemoteMessageRelay
{
    public Queue<object> MessageQueue { get; } = new();

    public void HandleMessage<T>(T message)
    {
        if (message is null)
            throw new NullReferenceException("Message was null");

        MessageQueue.Enqueue(message);
    }
}