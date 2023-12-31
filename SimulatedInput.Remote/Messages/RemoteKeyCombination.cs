﻿using System.Text.Json.Serialization;
using SimulatedInput.Core.Enum;

namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteKeyCombination
{
    public VirtualKeyCode[] KeyCodes { get; }

    [JsonConstructor]
    public RemoteKeyCombination(VirtualKeyCode[] keyCodes)
    {
        KeyCodes = keyCodes;
    }
}