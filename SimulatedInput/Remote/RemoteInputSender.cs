using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using SimulatedInput.Remote.Messages.Events;

namespace SimulatedInput.Remote;

// TODO: Relay events from RemoteConnectionManager and RemoteConnection objects.
public class RemoteInputSender
{
    private readonly RemoteConnectionManager _connectionManager;
    private readonly List<RemoteConnection> _connections;

    public IPEndPoint EndPoint { get; }
    public ReadOnlyCollection<RemoteConnection> Connections { get; }

    public RemoteInputSender(IPEndPoint endPoint) : this
        (new RemoteMessageDeserializer(), new RemoteMessageRelay(), endPoint)
    {
    }

    public RemoteInputSender(IRemoteMessageDeserializer deserializer, IRemoteMessageRelay relay, IPEndPoint endPoint)
    {
        _connectionManager = new RemoteConnectionManager(deserializer, relay, endPoint);
        _connectionManager.ClientConnected += OnClientConnected;
        _connections = new List<RemoteConnection>();
        EndPoint = endPoint;
        Connections = new ReadOnlyCollection<RemoteConnection>(_connections);
    }

    public void StartRemoteListening()
    {
        _connectionManager.StartListening();
    }

    public void StopRemoteListening()
    {
        _connectionManager.StopListening();
    }

    private void OnClientConnected(object? sender, RemoteConnection e)
    {
        e.BadMessageReceived += OnBadMessageReceived;
        e.UnknownMessageReceived += OnUnknownMessageReceived;
        e.UnrecoverableException += OnUnrecoverableException;
        _connections.Add(e);
    }

    private void OnBadMessageReceived(object? sender, BadMessageEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnUnknownMessageReceived(object? sender, UnknownMessageEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    private void OnUnrecoverableException(object? sender, Exception e)
    {
        throw new NotImplementedException();
    }
}