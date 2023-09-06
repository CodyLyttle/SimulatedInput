namespace SimulatedInput.Remote;

public interface IRemoteMessageRelay
{
    void HandleMessage<T>(T message);
}