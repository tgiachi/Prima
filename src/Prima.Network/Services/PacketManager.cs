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

        _logger.LogInformation(
            "Registered packet: {Packet} with opCode: {opCode}",
            packet.GetType().Name,
            "0x" + packet.OpCode.ToString("X2")
        );
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

        // Write the packet data
        packetWriter.Write(packetData);

        return packetWriter.ToArray();
    }

    /// <summary>
    /// Deserializes a byte array into a list of network packets.
    /// </summary>
    /// <param name="data">The byte array containing the packet data.</param>
    /// <returns>A list of deserialized packets, or an empty list if no valid packets could be parsed.</returns>
    public List<IUoNetworkPacket> ReadPackets(byte[] data)
    {
        List<IUoNetworkPacket> packets = new();

        // Create a copy of the original buffer to work with
        var buffer = new Memory<byte>(data);

        while (buffer.Length > 0)
        {
            // Check if we have at least enough data for OpCode
            if (buffer.Length < 1)
            {
                break;
            }

            var opCode = buffer.Span[0];

            if (!_packets.TryGetValue(opCode, out var packetFunc))
            {
                _logger.LogWarning("Packet with OpCode {OpCode} is not registered.", opCode.ToString("X2"));
                break; // We can't continue if we don't know the packet type
            }

            var packet = packetFunc();

            // Check if we have enough data for this entire packet
            int expectedPacketLength = packet.Length;

            // If Length is -1, it means variable length packet that needs to be parsed
            // to determine its actual length
            if (expectedPacketLength == -1)
            {
                // For variable length packets, we need at least 3 bytes (OpCode + Length field)
                if (buffer.Length < 3)
                {
                    _logger.LogWarning("Buffer too small for variable length packet header: {Length} bytes", buffer.Length);
                    break;
                }

                // Read the length from the packet (assuming it's at bytes 1-2)
                ushort length = (ushort)((buffer.Span[1] << 8) | buffer.Span[2]);
                expectedPacketLength = 3 + length; // OpCode(1) + LengthField(2) + Payload(length)
            }

            // Check if we have enough data for the entire packet
            if (buffer.Length < expectedPacketLength)
            {
                _logger.LogWarning(
                    "Packet data truncated. Expected length: {Expected}, Actual available: {Actual}",
                    expectedPacketLength,
                    buffer.Length
                );
                break;
            }

            // Extract the packet data
            var packetData = buffer[..expectedPacketLength].ToArray();

            // Process the packet
            using (var packetReader = new PacketReader(packetData, expectedPacketLength, true))
            {
                try
                {
                    // Skip OpCode as it's already been read
                    //packetReader.ReadByte();

                    // If it's a variable length packet, skip the length bytes too
                    if (packet.Length == -1)
                    {
                        packetReader.ReadUInt16BE();
                    }

                    // Read the packet content
                    packet.Read(packetReader);
                    packets.Add(packet);

                    _logger.LogDebug("Successfully parsed packet: {PacketType}", packet.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing packet with OpCode {OpCode}", opCode.ToString("X2"));
                }
            }

            // Remove the processed packet from the buffer
            buffer = buffer[expectedPacketLength..];
        }

        return packets;
    }
}
