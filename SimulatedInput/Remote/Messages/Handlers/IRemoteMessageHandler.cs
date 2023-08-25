namespace SimulatedInput.Remote.Messages.Handlers;

public interface IRemoteMessageHandler<in T>
{
    void Handle(T message);
}