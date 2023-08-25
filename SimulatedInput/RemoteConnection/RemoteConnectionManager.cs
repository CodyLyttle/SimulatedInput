using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimulatedInput.RemoteConnection;

public class RemoteConnectionManager : IDisposable
{
    private readonly TcpListener _listener;
    private CancellationTokenSource? _listenCts;
    private Task? _listenTask;

    public bool IsListening => _listenTask != null && _listenCts is {IsCancellationRequested: false};
    
    public RemoteConnectionManager(IPEndPoint localEndpoint)
    {
        _listener = new TcpListener(localEndpoint);
    }
    
    public event EventHandler<Socket>? ClientConnected;
    public event EventHandler<Exception>? ListenerException;

    public void StartListening()
    {
        if (IsListening)
            throw new InvalidOperationException("Already listening.");

        _listener.Start();
        _listenCts = new CancellationTokenSource();
        CancellationToken token = _listenCts.Token;
        
        _listenTask = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    Socket client = await _listener.AcceptSocketAsync(token);
                    ClientConnected?.Invoke(this, client);
                }
            }
            catch (TaskCanceledException)
            {
                // Expected exception.
            }
            catch (Exception e)
            {
                // Unexpected exception.
                ListenerException?.Invoke(this, e);
            }
            finally
            {
                _listener.Stop();
            }
        }, token); 
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
        Debug.Assert(_listenCts != null && _listenTask != null);
        
        _listenCts.Cancel();
        await _listenTask;
        _listenCts.Dispose();
        _listenCts = null;
    }

    public void Dispose()
    {
        _listenCts?.Dispose();
    }
}