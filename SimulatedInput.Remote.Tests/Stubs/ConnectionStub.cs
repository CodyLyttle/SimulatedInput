namespace SimulatedInput.Remote.Tests.Stubs;

public class ConnectionStub : IDisposable
{
    public bool IsConnected { get; set; }
    public Queue<byte> Buffer { get; } = new();

    public void Dispose()
    {
    }
}