using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimulatedInput.Remote.Messages;
using SimulatedInput.Remote.Messages.Events;

namespace SimulatedInput.Remote;

public class RemoteConnection : IDisposable
{
    private CancellationTokenSource? _receiveMessageCts;
    private Task? _receiveMessageTask;
    private readonly IRemoteMessageDeserializer _deserializer;
    private readonly IRemoteMessageRelay _relay;
    private readonly Socket _remoteClient;
    public event EventHandler<Exception>? UnrecoverableException;
    public event EventHandler<BadMessageEventArgs>? BadMessageReceived;
    public event EventHandler<UnknownMessageEventArgs>? UnknownMessageReceived;

    public bool IsListening => _remoteClient.Connected
                               && _receiveMessageTask != null
                               && _receiveMessageCts is {IsCancellationRequested: false};

    public RemoteConnection(IRemoteMessageDeserializer deserializer,
        IRemoteMessageRelay relay,
        Socket remoteClient)
    {
        _deserializer = deserializer;
        _relay = relay;
        _remoteClient = remoteClient;
    }

    public void StartListening()
    {
        if (IsListening)
            throw new InvalidOperationException("Already listening.");

        _receiveMessageCts = new CancellationTokenSource();
        _receiveMessageTask = ReceiveMessageLoopAsync(_receiveMessageCts.Token);
        _receiveMessageTask.Start();
    }

    public void StopListening()
    {
        if (!IsListening)
            throw new InvalidOperationException("Not listening.");

        // Fire and forget.
        Task.Run(CancelListenTask);
    }

    public async Task StopListeningAsync()
    {
        if (!IsListening)
            throw new InvalidOperationException("Not listening.");

        await CancelListenTask();
    }

    private async Task CancelListenTask()
    {
        // Handled by IsListening calls in public methods.
        Debug.Assert(_receiveMessageCts != null && _receiveMessageTask != null);

        _receiveMessageCts.Cancel();
        await _receiveMessageTask;
        _receiveMessageCts.Dispose();
        _receiveMessageCts = null;
    }

    // TODO: Handle client connection closed.
    // 1) When ReceiveAsync returns 0 bytes. (graceful)
    // 2) When SocketException occurs. (forced or unexpected)
    private async Task ReceiveMessageLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Listen for message length and type.
            var prefixBuffer = new byte[8];
            await _remoteClient.ReceiveAsync(prefixBuffer, SocketFlags.None, token);

            var msgSize = BitConverter.ToInt32(prefixBuffer);
            if (msgSize < 0)
            {
                _remoteClient.Close();

                Exception ex =
                    new("Received a message of negative size, the remote client connection has been closed.");
                UnrecoverableException?.Invoke(this, ex);
                throw ex;
            }

            RemoteMessageType msgType = (RemoteMessageType) BitConverter.ToInt32(prefixBuffer, 4);

            var msgBuffer = new byte[msgSize];
            int remainingBytes = msgSize;
            while (remainingBytes > 0 && !token.IsCancellationRequested)
            {
                int nextEmptyByte = msgSize - remainingBytes;
                Memory<byte> virtualBuffer = new(msgBuffer, nextEmptyByte, remainingBytes);
                remainingBytes -= await _remoteClient.ReceiveAsync(virtualBuffer, SocketFlags.None, token);
            }

            if (token.IsCancellationRequested)
                break;

            if (Enum.IsDefined(msgType))
            {
                ProcessMessage(msgType, msgBuffer);
            }
            else
            {
                UnknownMessageReceived?.Invoke(this,
                    new UnknownMessageEventArgs((int) msgType, msgBuffer));
            }
        }

        throw new TaskCanceledException();
    }

    private void ProcessMessage(RemoteMessageType msgType, byte[] msgContent)
    {
        switch (msgType)
        {
            case RemoteMessageType.KeyCombination:
                DeserializeAndHandle<RemoteKeyCombination>(msgType, msgContent);
                break;
            case RemoteMessageType.MouseButton:
                DeserializeAndHandle<RemoteMouseButton>(msgType, msgContent);
                break;
            case RemoteMessageType.MouseMove:
                DeserializeAndHandle<RemoteMouseMove>(msgType, msgContent);
                break;
            case RemoteMessageType.Scroll:
                DeserializeAndHandle<RemoteScroll>(msgType, msgContent);
                break;
            case RemoteMessageType.Text:
                DeserializeAndHandle<RemoteText>(msgType, msgContent);
                break;
            default:
                // Unknown messages have already been caught by ReceiveMessageLoop.
                // Should only throw if enum is modified or expanded without making equivalent changes to this switch.
                throw new ArgumentOutOfRangeException(nameof(msgType), msgType,
                    "Unsupported message type");
        }
    }

    private void DeserializeAndHandle<T>(RemoteMessageType messageType, byte[] data)
    {
        try
        {
            T? message = _deserializer.Deserialize<T>(data);
            if (message is null)
            {
                BadMessageReceived?.Invoke(this,
                    new BadMessageEventArgs(messageType, data, "Deserialized to null"));
            }
            else
            {
                _relay.HandleMessage(message);
            }
        }
        catch (Exception e)
        {
            BadMessageReceived?.Invoke(this,
                new BadMessageEventArgs(messageType, data, e.Message));
        }
    }

    public void Dispose()
    {
        _remoteClient.Dispose();
    }
}