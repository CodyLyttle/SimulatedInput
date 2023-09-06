using System.Collections.ObjectModel;
using SimulatedInput.Remote.Messages;

namespace SimulatedInput.Remote;

public static class RemoteMessageLookup
{
    private static readonly ReadOnlyDictionary<RemoteMessageType, Type> EnumToTypeMap;
    private static readonly ReadOnlyDictionary<Type, RemoteMessageType> TypeToEnumMap;

    static RemoteMessageLookup()
    {
        Dictionary<RemoteMessageType, Type> map = new()
        {
            [RemoteMessageType.KeyCombination] = typeof(RemoteKeyCombination),
            [RemoteMessageType.MouseButton] = typeof(RemoteMouseButton),
            [RemoteMessageType.MouseMove] = typeof(RemoteMouseMove),
            [RemoteMessageType.Scroll] = typeof(RemoteScroll),
            [RemoteMessageType.Text] = typeof(RemoteText)
        };

        Dictionary<Type, RemoteMessageType> invertedMap = map.ToDictionary(pair => pair.Value, pair => pair.Key);

        // Assign as ReadOnlyDictionary's to prevent runtime mutation.
        EnumToTypeMap = new ReadOnlyDictionary<RemoteMessageType, Type>(map);
        TypeToEnumMap = new ReadOnlyDictionary<Type, RemoteMessageType>(invertedMap);
    }

    public static Type AsType(RemoteMessageType remoteMessageType)
    {
        if (!EnumToTypeMap.TryGetValue(remoteMessageType, out Type? messageType))
            throw new ArgumentOutOfRangeException(nameof(remoteMessageType), remoteMessageType, 
                $"Unmapped {nameof(RemoteMessageType)} '{remoteMessageType}'.");

        return messageType;
    }

    public static RemoteMessageType AsRemoteMessageType(Type messageType)
    {
        if (!TypeToEnumMap.TryGetValue(messageType, out RemoteMessageType messageTypeEnum))
            throw new ArgumentOutOfRangeException(nameof(messageType), messageType, 
                $"Unmapped {nameof(Type)} '{messageType}'.");

        return messageTypeEnum;
    }
}