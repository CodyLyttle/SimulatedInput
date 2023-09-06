namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteText
{
    public string Content { get; }

    public RemoteText(string content)
    {
        Content = content;
    }
}