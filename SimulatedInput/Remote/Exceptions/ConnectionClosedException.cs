using System;

namespace SimulatedInput.Remote.Exceptions;

public class ConnectionClosedException : Exception
{
    public bool ClosedGracefully { get; }

    public ConnectionClosedException(bool closedGracefully, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ClosedGracefully = closedGracefully;
    }
}