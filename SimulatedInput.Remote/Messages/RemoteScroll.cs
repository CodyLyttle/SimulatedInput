namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteScroll
{
    // Positive = scroll up.
    // Negative = scroll down.
    public int Delta { get; }

    public RemoteScroll(int delta)
    {
        Delta = delta;
    }
}