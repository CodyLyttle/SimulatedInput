using System.Text.Json.Serialization;

namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteMouseMove
{
    public int PositionX { get; }
    public int PositionY { get; }

    
    [JsonConstructor]
    public RemoteMouseMove(int posX, int posY)
    {
        PositionX = posX;
        PositionY = posY;
    }
}