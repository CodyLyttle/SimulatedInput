using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimulatedInput.Remote;

public class RemoteInputServer : IDisposable
{
    private bool _isDisposed;
    private CancellationTokenSource? _listenCts;
    private readonly object _lock = new();
    private readonly IRemoteMessageDeserializer _deserializer;
    private readonly IRemoteMessageRelay _relay;
    private readonly TcpListener _listener;
    private readonly List<IRemoteConnection> _connections;

    public ReadOnlyCollection<IRemoteConnection> Connections { get; }
    public IPEndPoint LocalEndPoint { get; }
    public bool IsReceivingConnections { get; private set; }

    public RemoteInputServer(IPEndPoint endpoint) 
        : this(endpoint, new RemoteMessageDeserializer(), new RemoteMessageRelay())
    {
    }
    
    public RemoteInputServer(IPEndPoint localEndpoint, 
        IRemoteMessageDeserializer deserializer,
        IRemoteMessageRelay relay)
    {
        LocalEndPoint = localEndpoint;
        _deserializer = deserializer;
        _relay = relay;
        
        _listener = new TcpListener(localEndpoint);
        _connections = new List<IRemoteConnection>();
        Connections = new ReadOnlyCollection<IRemoteConnection>(_connections);
    }

    public event EventHandler<IRemoteConnection>? ClientConnected;

    public void StartAcceptingConnections()
    {
        lock (_lock)
        {
            if (IsReceivingConnections)
                throw new InvalidOperationException(
                    $"{nameof(RemoteInputServer)} is already accepting connections.");

            _listener.Start();
            _listenCts = new CancellationTokenSource();
            IsReceivingConnections = true;
        }

        CancellationToken token = _listenCts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                Socket connection;
                try
                {
                    connection = await _listener.AcceptSocketAsync(token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An unexpected exception was thrown by AcceptSocketAsync: {ex}");
                    continue;
                }

                // Continue setting up the connection on another thread.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        Task<IRemoteConnection?> setupConnection = InitializeConnection(connection, token);
                        Task connectionTimeout = Task.Delay(TimeSpan.FromSeconds(5), token);
                        Task result = await Task.WhenAny(setupConnection, connectionTimeout);

                        if (result == connectionTimeout)
                        {
                            Debug.WriteLine("Connection timed out during handshake.");
                            return;
                        }

                        IRemoteConnection? newConnection = await setupConnection;
                        if (newConnection == null)
                        {
                            Debug.WriteLine("Connection of unknown type was rejected.");
                            return;
                        }

                        ClientConnected?.Invoke(this, newConnection);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"An unexpected exception was thrown by 'SetupNewConnection': {ex}");
                    }
                }, token);
            }

            if (token.IsCancellationRequested)
                throw new TaskCanceledException();
        }, token);
    }

    public void StopAcceptingConnections()
    {
        lock (_lock)
        {
            if (!IsReceivingConnections)
                throw new InvalidOperationException(
                    $"{nameof(RemoteInputServer)} is not accepting connections.");

            IsReceivingConnections = false;
            _listenCts!.Cancel();
            _listenCts.Dispose();
            _listenCts = null;
            _listener.Stop();
        }
    }

    public void CloseConnections()
    {
        foreach (IRemoteConnection conn in _connections)
        {
            conn.CloseConnection();
            conn.Dispose();
        }

        _connections.Clear();
    }

    protected virtual async Task<IRemoteConnection?> InitializeConnection(Socket connection, CancellationToken token)
    {
        string? connectionCode = await ReadConnectionIdentifierCode(connection, token);
        return connectionCode switch
        {
            "GET" => await InitializeWebSocket(_deserializer, _relay, connection, token),
            "TCP" => await InitializeSocket(_deserializer, _relay, connection, token),
            _ => null
        };
    }

    private async Task<string?> ReadConnectionIdentifierCode(Socket connection, CancellationToken token)
    {
        const int codeLength = 3;
        var buffer = new byte[codeLength];
        var cursor = 0;

        while (cursor < codeLength && !token.IsCancellationRequested)
        {
            Memory<byte> virtualBuffer = new(buffer, cursor, codeLength - cursor);
            try
            {
                int received = await connection.ReceiveAsync(virtualBuffer, token);
                if (received == 0)
                {
                    Debug.WriteLine("Connection terminated gracefully during handshake.");
                    return null;
                }

                cursor += received;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Connection terminated forcefully during handshake: " + ex.Message);
                return null;
            }
        }

        if (token.IsCancellationRequested)
            throw new TaskCanceledException();

        return Encoding.UTF8.GetString(buffer);
    }

    protected virtual Task<RemoteSocketConnection> InitializeSocket(
        IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay, Socket connection, CancellationToken token)
    {
        RemoteSocketConnection socket = new(deserializer, relay, connection);
        return Task.FromResult(socket);
    }

    protected virtual async Task<RemoteWebSocketConnection> InitializeWebSocket(
        IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay, Socket connection, CancellationToken token)
    {
        // TODO: Receive the rest of the GET message and complete the websocket upgrade.
        throw new NotImplementedException();
    }

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
                if (IsReceivingConnections)
                {
                    StopAcceptingConnections(); // Disposes token.
                }

                CloseConnections(); // Disposes connections.
            }

            _isDisposed = true;
        }
    }
}