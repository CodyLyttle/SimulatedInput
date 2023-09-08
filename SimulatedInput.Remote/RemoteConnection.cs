using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SimulatedInput.Remote.Exceptions;
using SimulatedInput.Remote.Messages;

namespace SimulatedInput.Remote;

public abstract class RemoteConnection<TConn> : IRemoteConnection
    where TConn : IDisposable
{
    private bool _isDisposed;
    private readonly object _lock = new();
    private readonly IRemoteMessageDeserializer _deserializer;
    private readonly IRemoteMessageRelay _relay;
    private readonly CancellationTokenSource _receiveCts = new();

    protected abstract bool IsConnected { get; }

    public TConn Connection { get; }

    private RemoteConnectionState? _state;

    public RemoteConnectionState State
    {
        get
        {
            _state ??= IsConnected
                ? RemoteConnectionState.Open
                : RemoteConnectionState.Closed;

            return _state.Value;
        }

        private set => _state = value;
    }

    protected RemoteConnection(IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay, TConn connection)
    {
        _deserializer = deserializer;
        _relay = relay;
        Connection = connection;
    }

    public event EventHandler<ConnectionClosedException>? ClosedByClient;
    public event EventHandler<MessageFormatException>? BadMessageFormat;
    public event EventHandler<MessageDeserializerException>? FailedToDeserialize;

    public void BeginReceivingMessages()
    {
        lock (_lock)
        {
            if (State != RemoteConnectionState.Open)
                throw new InvalidOperationException(
                    $"{nameof(BeginReceivingMessages)} can only be called when connection state is 'Open', " +
                    $"the current state is '{State}'.");

            State = RemoteConnectionState.Receiving;
        }

        CancellationToken token = _receiveCts.Token;
        Task.Run(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    byte[] messagePrefix = await ReadBytesAsync(Connection, 8, token);
                    var contentSize = BitConverter.ToInt32(messagePrefix);
                    if (contentSize < 0)
                    {
                        CloseConnection();
                        MessageFormatException sizeException = new("Message size was negative.");
                        NotifyMessageFormatException(sizeException);
                        throw sizeException;
                    }

                    RemoteMessageType messageType = (RemoteMessageType) BitConverter.ToInt32(messagePrefix, 4);
                    byte[] messageContent = await ReadBytesAsync(Connection, contentSize, token);

                    if (Enum.IsDefined(messageType))
                    {
                        ProcessMessage(messageType, messageContent);
                    }
                    else
                    {
                        MessageFormatException typeException = new($"Unknown message type: '{messageType}'.");
                        NotifyMessageFormatException(typeException);
                        throw typeException;
                    }
                }
            }
            catch (ConnectionClosedException closedException)
            {
                NotifyConnectionClosedException(closedException);
                throw;
            }

            throw new TaskCanceledException();
        }, token);
    }

    protected abstract Task<byte[]> ReadBytesAsync(TConn connection, int bytesToRead, CancellationToken token);


    private void ProcessMessage(RemoteMessageType msgType, byte[] msgContent)
    {
        switch (msgType)
        {
            case RemoteMessageType.KeyCombination:
                DeserializeAndHandle<RemoteKeyCombination>(msgContent);
                break;
            case RemoteMessageType.MouseButton:
                DeserializeAndHandle<RemoteMouseButton>(msgContent);
                break;
            case RemoteMessageType.MouseMove:
                DeserializeAndHandle<RemoteMouseMove>(msgContent);
                break;
            case RemoteMessageType.Scroll:
                DeserializeAndHandle<RemoteScroll>(msgContent);
                break;
            case RemoteMessageType.Text:
                DeserializeAndHandle<RemoteText>(msgContent);
                break;
            default:
            {
                MessageFormatException typeException = new($"Unknown message type: '{msgType}'.");
                NotifyMessageFormatException(typeException);
                throw typeException;
            }
        }
    }

    private void DeserializeAndHandle<T>(byte[] data)
    {
        T? message;
        
        try
        {
            message = _deserializer.Deserialize<T>(data);
            if (message is null)
            {
                MessageDeserializerException nullException = new("Message deserialized as null.");
                NotifyDeserializerException(nullException);
                throw nullException;
            }
        }
        catch (Exception ex)
        {
            MessageDeserializerException wrappedException = new(ex.Message, ex);
            NotifyDeserializerException(wrappedException);
            throw wrappedException;
        }

        // Relay shouldn't throw, although an uncaught exception would cause the receive message loop to fail silently.
        try
        {
            _relay.HandleMessage(message);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    public void CloseConnection()
    {
        lock (_lock)
        {
            if (State is not RemoteConnectionState.Open or RemoteConnectionState.Receiving)
                throw new InvalidOperationException($"Connection is already {State}.");

            State = RemoteConnectionState.Closing;

            try
            {
                _receiveCts.Cancel();
            }
            catch (Exception e)
            {
                LogException(e);
            }

            try
            {
                DoCloseConnection();
            }
            catch (Exception e)
            {
                LogException(e);
            }

            State = RemoteConnectionState.Closed;

            void LogException(Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }

    protected abstract void DoCloseConnection();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        lock (_lock)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (State is RemoteConnectionState.Open or RemoteConnectionState.Receiving)
                {
                    CloseConnection();
                }
            
                Connection.Dispose();
                _receiveCts.Dispose();
            }
         
            _isDisposed = true;
        }
    }

    private void NotifyConnectionClosedException(ConnectionClosedException e)
    {
        ClosedByClient?.Invoke(this, e);
    }

    private void NotifyMessageFormatException(MessageFormatException e)
    {
        BadMessageFormat?.Invoke(this, e);
    }

    private void NotifyDeserializerException(MessageDeserializerException e)
    {
        FailedToDeserialize?.Invoke(this, e);
    }
}