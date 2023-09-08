using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SimulatedInput.Remote.Messages;
using SimulatedInput.Remote.Tests.Stubs;

namespace SimulatedInput.Remote.Tests.Integration;

// TODO: Send bad messages.
// eg. < 8 bytes,
//     unknown message type.
//     length mismatch.
public class RemoteInputServerTests
{
    private class ConnectionTester<T> where T : IRemoteConnection
    {
        private readonly MessageRelayStub _relayStub;
        private readonly RemoteInputServer _server;
        public List<T> Connections { get; } = new();

        public ConnectionTester(int port)
        {
            _relayStub = new MessageRelayStub();

            _server = new RemoteInputServer(
                new IPEndPoint(IPAddress.Any, port),
                new RemoteMessageDeserializer(),
                _relayStub);

            _server.ClientConnected += (_, connection) => { Connections.Add((T) connection); };
            _server.StartAcceptingConnections();
        }

        public void AssertConnectionCount(int expectedConnectionCount)
        {
            Assert.Equal(expectedConnectionCount, Connections.Count);
        }

        public void AssertMessageCount(int expectedMessageCount)
        {
            Assert.Equal(expectedMessageCount, _relayStub.MessageQueue.Count);
        }

        public void AssertQueuedMessage<TMsg>(TMsg expectedMessage)
        {
            object actualMessage = _relayStub.MessageQueue.Dequeue();
            Assert.IsType<TMsg>(actualMessage);

            string expectedJson = JsonSerializer.Serialize(expectedMessage);
            string actualJson = JsonSerializer.Serialize(actualMessage);
            Assert.Equal(expectedJson, actualJson);
        }
    }

    private const int Port = 12599;

    private readonly MessageRelayStub _relayStub;
    private readonly RemoteInputServer _server;

    public RemoteInputServerTests()
    {
        _relayStub = new MessageRelayStub();

        _server = new RemoteInputServer(
            new IPEndPoint(IPAddress.Any, Port),
            new RemoteMessageDeserializer(),
            _relayStub);
    }

    [Fact]
    public async Task SendSocketMessage()
    {
        // Note: Task.Delay() is used to allow time for messages to be received and processed by the server.

        // Arrange
        const string handshakeCode = "TCP";
        ConnectionTester<RemoteSocketConnection> tester = new(Port);
        TcpClient client = new TcpClient();
        RemoteText message = new("MyMessage");
        byte[] messageData = GetSerializedMessage(message);

        // Act
        await client.ConnectAsync("localhost", Port);
        Stream stream = client.GetStream();
        await stream.WriteAsync(Encoding.UTF8.GetBytes(handshakeCode));
        await stream.WriteAsync(messageData);
        await Task.Delay(500);

        // Assert
        tester.AssertConnectionCount(1);
        tester.AssertMessageCount(1);
        tester.AssertQueuedMessage(message);

        client.Close();
    }

    [Fact]
    public async Task SendMessageOverWebSocket()
    {
        // Arrange
        ConnectionTester<RemoteWebSocketConnection> tester = new(Port);
        ClientWebSocket webSocket = new();
        Uri connectionUri = new($"ws://localhost:{Port}");
        CancellationTokenSource cts = new CancellationTokenSource();
        RemoteText message = new("MyMessage");
        byte[] messageData = GetSerializedMessage(message);

        // Act
        await webSocket.ConnectAsync(connectionUri, cts.Token);
        Debug.WriteLine(webSocket.State);
        await webSocket.SendAsync(messageData, WebSocketMessageType.Binary, true, cts.Token);
        await Task.Delay(500);

        // Assert
        tester.AssertConnectionCount(1);
        tester.AssertMessageCount(1);
        tester.AssertQueuedMessage(message);

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cts.Token);
    }

    public byte[] GetSerializedMessage<T>(T message)
    {
        RemoteMessageType msgType = RemoteMessageLookup.AsRemoteMessageType(typeof(T));
        byte[] msgContent = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        int bufferSize = 8 + msgContent.Length;
        using MemoryStream stream = new(bufferSize);
        stream.Write(BitConverter.GetBytes(msgContent.Length));
        stream.Write(BitConverter.GetBytes((int) msgType));
        stream.Write(msgContent);

        return stream.GetBuffer();
    }
}