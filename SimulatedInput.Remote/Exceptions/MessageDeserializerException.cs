using System;

namespace SimulatedInput.Remote.Exceptions;

public class MessageDeserializerException : Exception
{
    public MessageDeserializerException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}