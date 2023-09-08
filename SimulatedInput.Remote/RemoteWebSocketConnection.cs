using System.Net.WebSockets;
using SimulatedInput.Remote.Exceptions;

namespace SimulatedInput.Remote;

public class RemoteWebSocketConnection : RemoteConnection<WebSocket>
{
    protected override bool IsConnected => Connection.State == WebSocketState.Open;
    public bool IsClosing { get; private set; }

    public RemoteWebSocketConnection(IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay,
        WebSocket connection) : base(deserializer, relay, connection)
    {
    }

    protected override async Task<byte[]> ReadBytesAsync(WebSocket connection, int bytesToRead, CancellationToken token)
    {
        var buffer = new byte[bytesToRead];
        var cursor = 0;
        while (cursor < bytesToRead && !token.IsCancellationRequested)
        {
            Memory<byte> bufferSlice = new(buffer, cursor, buffer.Length - cursor);
            ValueWebSocketReceiveResult result;

            try
            {
                result = await connection.ReceiveAsync(bufferSlice, token);
            }
            catch (WebSocketException e)
            {
                throw new ConnectionClosedException(false, "Web socket connection closed forcefully.", e);
            }

            if (result.MessageType == WebSocketMessageType.Binary)
            {
                cursor += result.Count;
                continue;
            }

            // TODO: Force close underlying connection if timeout during closing handshake.
            CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(3));
            CancellationToken timeoutToken = timeoutCts.Token;

            if (result.MessageType == WebSocketMessageType.Text)
            {
                await Connection.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Binary data expected",
                    timeoutToken);

                throw new ConnectionClosedException(false,
                    "Web socket client sent 'Text' instead of 'Binary' and has been closed");
            }


            await Connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server received close frame",
                timeoutToken);

            throw new ConnectionClosedException(true, "Web socket connection closed gracefully.");
        }

        token.ThrowIfCancellationRequested();
        return buffer;
    }

    // TODO: Update the interface to make this asynchronous.
    protected override void DoCloseConnection()
    {
        if (!IsConnected)
            throw new InvalidOperationException(
                $"Web socket cannot be closed in it's current state: '{Connection.State}'");

        if (IsClosing)
            throw new InvalidOperationException("Web socket is already in the process of closing.");

        IsClosing = true;

        Task.Run(async () =>
        {
            await Connection.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            IsClosing = false;
        });
    }
}