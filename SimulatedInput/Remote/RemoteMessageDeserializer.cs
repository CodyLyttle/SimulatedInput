﻿using System.Text.Json;

namespace SimulatedInput.Remote;

public class RemoteRemoteMessageDeserializer : IRemoteMessageDeserializer
{
    public T? Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data);
    }
}