using System.Text.Json.Serialization;

namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteText
{
    public string Content { get; }

    [JsonConstructor]
    public RemoteText(string content)
    {
        Content = content;
    }
}