using Microsoft.Extensions.Logging;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Interfaces.Services;
using Prima.Network.Serializers;

namespace Prima.Network.Services;

public class PacketManager : IPacketManager
{
    private readonly ILogger _logger;

    private readonly Dictionary<byte, Func<IUoNetworkPacket>> _packets = new();

    public PacketManager(ILogger<PacketManager> logger)
    {
        _logger = logger;
    }

    public void RegisterPacket<T>() where T : IUoNetworkPacket, new()
    {
        var packet = new T();
        var func = new Func<IUoNetworkPacket>(() => new T());
        if (!_packets.TryAdd(packet.OpCode, func))
        {
            _logger.LogWarning("Packet with OpCode {OpCode} is already registered.", packet.OpCode);
            return;
        }

        _logger.LogInformation("Registered packet: {Packet}", packet.GetType().Name);
    }

    public byte[] WritePacket<T>(T packet) where T : IUoNetworkPacket
    {
        using var memoryStream = new MemoryStream();
        using var packetWriter = new PacketWriter();
        using var writer = new BinaryWriter(memoryStream);

        writer.Write(packet.OpCode);

        packet.Write(packetWriter);
        var packetData = packetWriter.ToArray();

        writer.Write((byte)packetData.Length);
        writer.Write(packetData);

        return memoryStream.ToArray();
    }

    public IUoNetworkPacket ReadPacket(byte[] data)
    {
        var opCode = data[0];

        if (_packets.TryGetValue(opCode, out var packetFunc))
        {
            var packet = packetFunc();
            using var memoryStream = new MemoryStream(data);
            using var reader = new BinaryReader(memoryStream);

            reader.ReadByte();
            var length = reader.ReadByte();
            var packetData = reader.ReadBytes(length);

            using var packetReader = new PacketReader(packetData);
            packet.Read(packetReader);

            return packet;
        }
        else
        {
            _logger.LogWarning("Packet with OpCode {OpCode} is not registered.", opCode);
            return null;
        }
    }
}
