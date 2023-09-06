using System.Net;
using System.Net.Sockets;
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
    private const int Port = 12599;
    
    [Fact]
    public async Task TestSocketConnection()
    {
        // Note: Task.Delay() is used to allow time for messages to be received and processed by the server.
        
        MessageRelayStub relayStub = new();
        
        RemoteInputServer server = new (
            new IPEndPoint(IPAddress.Any, Port),
            new RemoteMessageDeserializer(),
            relayStub);
        
        var connectionCount = 0;
        server.ClientConnected += (_, _) => connectionCount++;
        server.StartAcceptingConnections();

        // Assert server accepts TCP connections.
        using TcpClient tcpConnection = await ConnectAsTcpClient();
        await Task.Delay(500);
        Assert.Equal(1, connectionCount);
        
        // Assert server receives and processes messages.        
        RemoteText message = new("MyMessage");
        await SendMessage(tcpConnection.GetStream(), message);
        await Task.Delay(500);

        // Assert message of the same type was received.
        Assert.Equal(1, relayStub.MessageQueue.Count);
        Assert.IsType<RemoteText>(relayStub.MessageQueue.Peek());
        
        // Assert message with matching contents was received.
        var receivedMessage = (RemoteText) relayStub.MessageQueue.Dequeue();
        AssertEqualMessages(message, receivedMessage);
        
        tcpConnection.Close();
    }

    private async Task<TcpClient> ConnectAsTcpClient()
    {
        const string tcpHandshakeCode = "TCP";
        
        // Connect.
        TcpClient client = new TcpClient();
        await client.ConnectAsync("localhost", Port);
        
        // Handshake.
        NetworkStream stream = client.GetStream();
        await stream.WriteAsync(Encoding.UTF8.GetBytes(tcpHandshakeCode));

        return client;
    }

    private async Task SendMessage<T>(NetworkStream stream, T message)
    {
        // Encode message.
        string json = JsonSerializer.Serialize(message);
        byte[] msgContent = Encoding.UTF8.GetBytes(json);
        byte[] msgLength = BitConverter.GetBytes(msgContent.Length);
        byte[] msgType = BitConverter.GetBytes((int) RemoteMessageLookup.AsRemoteMessageType(typeof(T)));

        // Send.
        await stream.WriteAsync(msgLength);
        await stream.WriteAsync(msgType);
        await stream.WriteAsync(msgContent);
    }

    private void AssertEqualMessages<T>(T sent, T received)
    {
        string sentJson = JsonSerializer.Serialize(sent);
        string receivedJson = JsonSerializer.Serialize(received);
        
        Assert.Equal(sentJson, receivedJson);
    }
}