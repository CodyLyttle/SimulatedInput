namespace SimulatedInput.Remote.Tests.Stubs;

public class RemoteConnectionStub : RemoteConnection<ConnectionStub>
{
    public RemoteConnectionStub(IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay,
        ConnectionStub connection) : base(deserializer, relay, connection)
    {
    }

    protected override bool IsConnected => Connection.IsConnected;

    protected override async Task<byte[]> ReadBytesAsync(ConnectionStub connection, int bytesToRead,
        CancellationToken token)
    {
        List<byte> readBytes = new();
        while (bytesToRead > 0)
        {
            while (connection.Buffer.Count == 0)
            {
                await Task.Delay(500, token);
            }

            readBytes.Add(connection.Buffer.Dequeue());
            bytesToRead--;
        }

        return readBytes.ToArray();
    }

    protected override void DoCloseConnection()
    {
        Connection.Dispose();
    }
}