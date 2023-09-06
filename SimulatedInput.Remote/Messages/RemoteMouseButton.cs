using SimulatedInput.Core.Enum;

namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteMouseButton
{
    public MouseButton Button { get; }
    public bool IsUp { get; }

    public RemoteMouseButton(MouseButton button, bool isUp)
    {
        Button = button;
        IsUp = isUp;
    }
}