using System;

namespace SimulatedInput.Remote.Exceptions;

public class MessageFormatException : Exception
{
    public MessageFormatException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}