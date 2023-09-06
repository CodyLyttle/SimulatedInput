namespace SimulatedInput.Remote;

public interface IRemoteMessageDeserializer
{
    T? Deserialize<T>(byte[] data);
}