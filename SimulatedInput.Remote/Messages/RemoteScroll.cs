using System.Text.Json.Serialization;

namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteScroll
{
    // Positive = scroll up.
    // Negative = scroll down.
    public int Delta { get; }

    [JsonConstructor]
    public RemoteScroll(int delta)
    {
        Delta = delta;
    }
}