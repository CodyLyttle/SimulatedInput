using SimulatedInput.Core.Enum;

namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteKeyCombination
{
    public VirtualKeyCode[] KeyCodes { get; }

    public RemoteKeyCombination(VirtualKeyCode[] keyCodes)
    {
        KeyCodes = keyCodes;
    }
}