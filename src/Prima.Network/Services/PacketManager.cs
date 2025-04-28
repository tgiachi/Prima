using Microsoft.Extensions.Logging;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Interfaces.Services;
using Prima.Network.Serializers;

namespace Prima.Network.Services;

/// <summary>
/// Implementation of the IPacketManager interface.
/// Manages packet registration, serialization, and deserialization.
/// </summary>
public class PacketManager : IPacketManager
{
    /// <summary>
    /// Logger for this class.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Dictionary mapping OpCodes to packet factory functions.
    /// </summary>
    private readonly Dictionary<byte, Func<IUoNetworkPacket>> _packets = new();

    /// <summary>
    /// Initializes a new instance of the PacketManager class.
    /// </summary>
    /// <param name="logger">The logger for this class.</param>
    public PacketManager(ILogger<PacketManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers a packet type with the packet manager.
    /// </summary>
    /// <typeparam name="T">The type of packet to register.</typeparam>
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

    /// <summary>
    /// Serializes a packet to a byte array.
    /// </summary>
    /// <typeparam name="T">The type of packet to serialize.</typeparam>
    /// <param name="packet">The packet to serialize.</param>
    /// <returns>A byte array containing the serialized packet data.</returns>
    public byte[] WritePacket<T>(T packet) where T : IUoNetworkPacket
    {
        using var packetWriter = new PacketWriter();

        // Write OpCode
        packetWriter.Write(packet.OpCode);

        // Create a separate writer for the packet data
        using var dataWriter = new PacketWriter();
        packet.Write(dataWriter);
        var packetData = dataWriter.ToArray();

        // Write packet data length (as ushort to support larger packets)
        packetWriter.WriteUInt16BE((ushort)packetData.Length);

        // Write the packet data
        packetWriter.Write(packetData);

        return packetWriter.ToArray();
    }

    /// <summary>
    /// Deserializes a byte array into a packet.
    /// </summary>
    /// <param name="data">The byte array containing the packet data.</param>
    /// <returns>The deserialized packet, or null if the packet type is not registered.</returns>
    public IUoNetworkPacket ReadPacket(byte[] data)
    {
        if (data.Length < 3) // At minimum we need OpCode(1) + Length(2)
        {
            _logger.LogWarning("Packet data too small to contain a valid packet: {Length} bytes", data.Length);
            return null;
        }

        var opCode = data[0];

        if (_packets.TryGetValue(opCode, out var packetFunc))
        {
            var packet = packetFunc();
            using var reader = new PacketReader(data);

            // Read and verify OpCode
            var readOpCode = reader.ReadByte();
            if (readOpCode != opCode)
            {
                _logger.LogWarning("OpCode mismatch. Expected: {Expected}, Actual: {Actual}", opCode, readOpCode);
                return null;
            }

            // Read length (2 bytes)
            ushort length = reader.ReadUInt16BE();

            // Ensure we have enough data
            if (data.Length < 3 + length)
            {
                _logger.LogWarning(
                    "Packet data truncated. Expected length: {Expected}, Actual available: {Actual}",
                    length,
                    data.Length - 3
                );
                return null;
            }

            // Read the packet data
            byte[] packetData = reader.ReadBytes(length);

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
