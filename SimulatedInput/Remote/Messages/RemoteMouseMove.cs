namespace SimulatedInput.Remote.Messages;

public readonly struct RemoteMouseMove
{
    public int PositionX { get; }
    public int PositionY { get; }

    public RemoteMouseMove(int posX, int posY)
    {
        PositionX = posX;
        PositionY = posY;
    }
}