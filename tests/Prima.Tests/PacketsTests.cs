using Microsoft.Extensions.Logging.Abstractions;
using Prima.Network.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Network.Services;

namespace Prima.Tests;

public class PacketsTests
{

    private IPacketManager _packetManager;

    [SetUp]
    public void Setup()
    {
        _packetManager = new PacketManager(NullLogger<PacketManager>.Instance);
        _packetManager.RegisterPacket<LoginRequest>();
        _packetManager.RegisterPacket<LoginDenied>();
    }

    [Test]
    public void CreateLoginRequestPacket()
    {
        var loginRequest = new LoginRequest()
        {
            Command = 0x01,
            Username = "TestUser",
            Password = "TestPassword",
            NextLoginKey = 0x02
        };

        var packetData = _packetManager.WritePacket(loginRequest);

        Assert.That(packetData, Is.Not.Null);
        Assert.That(packetData, Has.Length.EqualTo(64));

    }

    [Test]
    public void CreateLoginRequestPacketAndReadIt()
    {
        var loginRequest = new LoginRequest()
        {
            Command = 0x01,
            Username = "TestUser",
            Password = "TestPassword",
            NextLoginKey = 0x02
        };

        var packetData = _packetManager.WritePacket(loginRequest);

        Assert.That(packetData, Is.Not.Null);
        Assert.That(packetData, Has.Length.EqualTo(64));

        var readPacket = _packetManager.ReadPacket(packetData);

        Assert.That(readPacket, Is.Not.Null);
        Assert.That(readPacket, Is.InstanceOf<LoginRequest>());
    }
}
