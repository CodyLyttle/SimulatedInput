using FluentAssertions;
using SimulatedInput.Remote.Tests.Stubs;

namespace SimulatedInput.Remote.Tests.Unit;

public class RemoteConnectionTests
{
    [Fact]
    public void CloseConnection_ShouldNotThrow_WhenStateOpen()
    {
        // Arrange
        IRemoteConnection testTarget = CreateStub();
        Assert.Equal(RemoteConnectionState.Open, testTarget.State);
    }

    [Fact]
    public void CloseConnection_ShouldNotThrow_WhenStateReceiving()
    {
        // Arrange
        IRemoteConnection testTarget = CreateStub();
        testTarget.BeginReceivingMessages();
        Assert.Equal(RemoteConnectionState.Receiving, testTarget.State);
    }

    [Fact]
    public void CloseConnection_ShouldThrowInvalidOperationException_WhenStateClosed()
    {
        // Arrange
        IRemoteConnection testTarget = CreateStub();
        testTarget.BeginReceivingMessages();
        testTarget.CloseConnection();
        Assert.Equal(RemoteConnectionState.Closed, testTarget.State);

        // Act/Assert
        Action act = () => testTarget.CloseConnection();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already closed*");
    }

    private static IRemoteConnection CreateStub()
    {
        ConnectionStub connectionStub = new() {IsConnected = true};
        RemoteConnectionStub connStub = new(new RemoteMessageDeserializer(), new RemoteMessageRelay(), connectionStub);
        return connStub;
    }
}