using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimulatedInput.Remote.Exceptions;

namespace SimulatedInput.Remote;

public class RemoteSocketConnection : RemoteConnection<Socket>
{
    protected override bool IsConnected => Connection.Connected;
    
    public RemoteSocketConnection(IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay, Socket connection) 
        : base(deserializer, relay, connection)
    {
    }

    protected override async Task<byte[]> ReadBytesAsync(Socket connection, int bytesToRead, CancellationToken token)
    {
        var buffer = new byte[bytesToRead];
        var cursor = 0;

        while (cursor < bytesToRead && !token.IsCancellationRequested)
        {
            Memory<byte> virtualBuffer = new(buffer, cursor, buffer.Length - cursor);
            var received = 0;
            try
            {
                received = await connection.ReceiveAsync(virtualBuffer, token);
            }
            catch(SocketException e)
            {
                throw new ConnectionClosedException(false, "Socket connection closed forcefully.", e);
            }

            if (received == 0)
            {
                throw new ConnectionClosedException(true, "Socket connection closed gracefully.");
            }

            cursor += received;
        }

        if (token.IsCancellationRequested)
            throw new TaskCanceledException();

        return buffer;
    }

    protected override void DoCloseConnection()
    {
        Connection.Close();
    }
}