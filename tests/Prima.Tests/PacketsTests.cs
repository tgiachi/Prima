using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Prima.Network.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Network.Packets.Entries;
using Prima.Network.Serializers;
using Prima.Network.Services;
using Prima.Network.Types;

namespace Prima.Tests;

/// <summary>
/// Test suite for network packet serialization and deserialization functionality.
/// Tests all packet types to ensure they correctly read and write data using NUnit 4.
/// </summary>
[TestFixture]
public class PacketTests
{
    private IPacketManager _packetManager;

    [SetUp]
    public void Setup()
    {
        // Setup the packet manager with a mock logger
        var loggerMock = new Mock<ILogger<PacketManager>>();
        _packetManager = new PacketManager(loggerMock.Object);

        // Register all packet types
        _packetManager.RegisterPacket<LoginRequest>();
        _packetManager.RegisterPacket<LoginDenied>();
        _packetManager.RegisterPacket<GameServerList>();
        _packetManager.RegisterPacket<SelectServer>();
        _packetManager.RegisterPacket<ConnectToGameServer>();
    }

    /// <summary>
    /// Tests the LoginRequest packet serialization and deserialization.
    /// </summary>
    [Test]
    public void LoginRequest_SerializeDeserialize_Success()
    {
        // Arrange
        var packet = new LoginRequest
        {
            Username = "TestUser",
            Password = "TestPassword",
            NextLoginKey = 0x01
        };

        // Act
        var serialized = _packetManager.WritePacket(packet);
        var deserialized = _packetManager.ReadPackets(serialized);

        var deserializedPacket = deserialized.FirstOrDefault(p => p is LoginRequest) as LoginRequest;

        // Assert
        Assert.That(deserializedPacket, Is.Not.Null);

        Assert.That(deserializedPacket.Username, Is.EqualTo(packet.Username));
        Assert.That(deserializedPacket.Password, Is.EqualTo(packet.Password));
        Assert.That(deserializedPacket.NextLoginKey, Is.EqualTo(packet.NextLoginKey));
    }

    /// <summary>
    /// Tests the LoginDenied packet serialization and deserialization.
    /// </summary>
    [Test]
    public void LoginDenied_SerializeDeserialize_Success()
    {
        // Arrange
        var packet = new LoginDenied
        {
            Reason = LoginDeniedReasonType.IncorrectPassword
        };

        // Act
        var serialized = _packetManager.WritePacket(packet);


        var deserialized = _packetManager.ReadPackets(serialized);

        var deserializedPacket = deserialized.FirstOrDefault(p => p is LoginDenied) as LoginDenied;

        // Assert
        Assert.That(deserializedPacket, Is.Not.Null);

        Assert.That(deserializedPacket.Reason, Is.EqualTo(packet.Reason));
    }

    // /// <summary>
    // /// Tests the GameServerList packet serialization and deserialization.
    // /// </summary>
    // [Test]
    // public void GameServerList_SerializeDeserialize_Success()
    // {
    //     // Arrange
    //     var packet = new GameServerList
    //     {
    //         SystemInfoFlag = 0x05
    //     };
    //
    //     var server1 = new GameServerEntry
    //     {
    //         Index = 1,
    //         Name = "TestServer1",
    //         LoadPercent = 50,
    //         TimeZone = 0,
    //         IP = IPAddress.Parse("127.0.0.1")
    //     };
    //
    //     var server2 = new GameServerEntry
    //     {
    //         Index = 2,
    //         Name = "TestServer2",
    //         LoadPercent = 30,
    //         TimeZone = 1,
    //         IP = IPAddress.Parse("192.168.1.1")
    //     };
    //
    //     packet.AddServer(server1);
    //     packet.AddServer(server2);
    //
    //     // Act
    //     var serialized = _packetManager.WritePacket(packet);
    //     var deserializedPacket = _packetManager.ReadPackets(serialized);
    //
    //     var deserialized = deserializedPacket.FirstOrDefault(p => p is GameServerList) as GameServerList;
    //
    //     // Assert
    //     Assert.That(deserialized, Is.Not.Null);
    //     Assert.That(deserialized.SystemInfoFlag, Is.EqualTo(packet.SystemInfoFlag));
    //
    //     Assert.That(deserialized.Servers.Count, Is.EqualTo(2));
    //
    //     // Check first server
    //     Assert.That(deserialized.Servers[0].Index, Is.EqualTo(server1.Index));
    //     Assert.That(deserialized.Servers[0].Name, Is.EqualTo(server1.Name));
    //     Assert.That(deserialized.Servers[0].LoadPercent, Is.EqualTo(server1.LoadPercent));
    //     Assert.That(deserialized.Servers[0].TimeZone, Is.EqualTo(server1.TimeZone));
    //     Assert.That(deserialized.Servers[0].IP.ToString(), Is.EqualTo(server1.IP.ToString()));
    //
    //     // Check second server
    //     Assert.That(deserialized.Servers[1].Index, Is.EqualTo(server2.Index));
    //     Assert.That(deserialized.Servers[1].Name, Is.EqualTo(server2.Name));
    //     Assert.That(deserialized.Servers[1].LoadPercent, Is.EqualTo(server2.LoadPercent));
    //     Assert.That(deserialized.Servers[1].TimeZone, Is.EqualTo(server2.TimeZone));
    //     Assert.That(deserialized.Servers[1].IP.ToString(), Is.EqualTo(server2.IP.ToString()));
    // }

    /// <summary>
    /// Tests the SelectServer packet serialization and deserialization.
    /// </summary>
    [Test]
    public void SelectServer_SerializeDeserialize_Success()
    {
        // Arrange
        var packet = new SelectServer
        {
            ShardId = 1
        };

        // Act
        var serialized = _packetManager.WritePacket(packet);
        var deserialized = _packetManager.ReadPackets(serialized);

        var deserializedPacket = deserialized.FirstOrDefault(p => p is SelectServer) as SelectServer;

        // Assert
        Assert.That(deserialized, Is.Not.Null);

        Assert.That(deserializedPacket.ShardId, Is.EqualTo(packet.ShardId));
    }

    /// <summary>
    /// Tests the ConnectToGameServer packet serialization and deserialization.
    /// </summary>
    [Test]
    public void ConnectToGameServer_SerializeDeserialize_Success()
    {
        // Arrange
        var packet = new ConnectToGameServer
        {
            GameServerIP = IPAddress.Parse("192.168.1.100"),
            GameServerPort = 2593,
            SessionKey = 0x12345678
        };

        // Act
        var serialized = _packetManager.WritePacket(packet);
        var deserializedPacket = _packetManager.ReadPackets(serialized);

        var deserialized = deserializedPacket.FirstOrDefault(p => p is ConnectToGameServer) as ConnectToGameServer;

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized.GameServerIP.ToString(), Is.EqualTo(packet.GameServerIP.ToString()));
        Assert.That(deserialized.GameServerPort, Is.EqualTo(packet.GameServerPort));
        Assert.That(deserialized.SessionKey, Is.EqualTo(packet.SessionKey));
    }


    [Test]
    public void ConnectToGameServer_Parse()
    {

        //                     8c    5f    8d     20   3a    0a    1e    cf    7f    8f    b5
        //var array = new byte[] { 0x8c, 0x5f, 0x8d, 0x20, 0x3a, 0x0a, 0x1e, 0xcf, 0x7f, 0x8f, 0xb5 };

        var array = new byte[] { 0x8c, 0x7F, 0x00, 0x00, 0x01, 0xA, 0x21, 0x43, 0x75, 0xEF, 0x25 };

        var deserializedPacket = _packetManager.ReadPackets(array);

        var deserialized = deserializedPacket.FirstOrDefault(p => p is ConnectToGameServer) as ConnectToGameServer;

        // Assert
        Assert.That(deserialized, Is.Not.Null);

        var connectToServer = new ConnectToGameServer();

        connectToServer.GameServerIP = IPAddress.Parse("127.0.0.1");
        connectToServer.GameServerPort = 2593;
        connectToServer.SessionKey = 1131802405;

        var serialized = _packetManager.WritePacket(connectToServer);

        Assert.That(serialized, Is.EqualTo(array));


    }

    [Test]
    public void GameListMessage_Parse()
    {
        byte[] array = new byte[]
        {
            0xa8, 0x00, 0x2e, 0x5d, 0x00, 0x01, 0x00, 0x00,
            0x4d, 0x6f, 0x64, 0x65, 0x72, 0x6e, 0x55, 0x4f,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x02, 0x01, 0x00, 0x00, 0x7f
        };


        var deserializedPacket = _packetManager.ReadPackets(array);

        var deserialized = deserializedPacket.FirstOrDefault(p => p is GameServerList) as GameServerList;

        Assert.That(deserialized, Is.Not.Null);

        var gameServerList = new GameServerList()
        {
            SystemInfoFlag = 0x5d,
        };

        gameServerList.AddServer(new GameServerEntry()
        {
            Index = 1,
            Name = "ModernUO",
            LoadPercent = 0,
            TimeZone = 2,
            IP = IPAddress.Parse("127.0.0.1")
        });

        var serialized = _packetManager.WritePacket(gameServerList);

        Assert.That(serialized, Is.EqualTo(array));

    }

    /// <summary>
    /// Tests the PacketReader and PacketWriter with various data types.
    /// </summary>
    [Test]
    public void PacketReaderWriter_DataTypes_Success()
    {
        // Arrange
        using var writer = new PacketWriter();

        // Write various data types
        writer.Write((byte)0x00);
        writer.Write((byte)0x01);
        writer.Write((ushort)0x0203);
        writer.Write((uint)0x04050607);
        writer.WriteAsciiFixed("Test", 10);
        writer.WriteAsciiNull("Variable");
        writer.WriteEnum(LoginDeniedReasonType.AccountBlocked);

        var dataArray = writer.ToArray();

        // Act
        using var reader = new PacketReader();

        reader.Initialize(dataArray, dataArray.Length, true);

        // Assert
        Assert.That(reader.ReadByte(), Is.EqualTo(0x01));
        Assert.That(reader.ReadUInt16BE(), Is.EqualTo(0x0203));
        Assert.That(reader.ReadUInt32BE(), Is.EqualTo(0x04050607u));
        Assert.That(reader.ReadFixedString(10), Is.EqualTo("Test"));
        Assert.That(reader.ReadString(), Is.EqualTo("Variable"));
        Assert.That(reader.ReadEnum<LoginDeniedReasonType>(), Is.EqualTo(LoginDeniedReasonType.AccountBlocked));
    }

    /// <summary>
    /// Tests that the PacketManager correctly handles unregistered packets.
    /// </summary>
    [Test]
    public void PacketManager_UnregisteredPacket_ReturnsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PacketManager>>();
        var packetManager = new PacketManager(loggerMock.Object);

        // Create a raw packet with unregistered OpCode
        var unknownPacketData = new byte[] { 0xFF, 0x01, 0x00 };

        // Act
        var result = packetManager.ReadPackets(unknownPacketData);

        // Assert
        Assert.That(result, Is.Empty);
    }

    // /// <summary>
    // /// Tests that a packet with maximum allowed servers (255) can be processed.
    // /// </summary>
    // [Test]
    // public void GameServerList_MaxServers_Success()
    // {
    //     // Arrange
    //     var packet = new GameServerList
    //     {
    //         SystemInfoFlag = 0x05,
    //     };
    //
    //     // Add 255 servers
    //     for (int i = 0; i < 10; i++)
    //     {
    //         packet.AddServer(
    //             new GameServerEntry
    //             {
    //                 Index = (ushort)i,
    //                 Name = $"Server{i}",
    //                 LoadPercent = (byte)(i % 100),
    //                 TimeZone = (byte)(i % 24),
    //                 IP = IPAddress.Parse($"192.168.0.{i % 255}")
    //             }
    //         );
    //     }
    //
    //     // Act
    //     var serialized = _packetManager.WritePacket(packet);
    //     var deserializedPackets = _packetManager.ReadPackets(serialized);
    //
    //     var deserialized = deserializedPackets.FirstOrDefault(p => p is GameServerList) as GameServerList;
    //
    //     // Assert
    //     Assert.That(deserialized, Is.Not.Null);
    //     Assert.That(deserialized.Servers.Count, Is.EqualTo(10));
    // }

    /// <summary>
    /// Tests that adding more than 255 servers doesn't exceed the limit.
    /// </summary>
    [Test]
    public void GameServerList_ExceedMaxServers_StaysAtLimit()
    {
        // Arrange
        var packet = new GameServerList
        {
            SystemInfoFlag = 0x05
        };

        // Try to add 260 servers (more than 255)
        for (int i = 0; i < 260; i++)
        {
            packet.AddServer(
                new GameServerEntry
                {
                    Index = (ushort)i,
                    Name = $"Server{i}",
                    LoadPercent = (byte)(i % 100),
                    TimeZone = (byte)(i % 24),
                    IP = IPAddress.Parse($"192.168.0.{i % 255}")
                }
            );
        }

        // Assert
        Assert.That(packet.Servers.Count, Is.EqualTo(255));
    }
}
