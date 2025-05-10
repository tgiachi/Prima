using Microsoft.Extensions.Logging;
using Orion.Foundations.Pool;
using Orion.Foundations.Spans;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Interfaces.Services;
using Prima.Network.Internal;


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


    // /// <summary>
    // ///  Object pool for reusing PacketReader instances.
    // /// </summary>
    // private readonly ObjectPool<SpanReader> _readerPool = new();
    //
    // /// <summary>
    // /// Object pool for reusing PacketWriter instances.
    // /// </summary>
    // private readonly ObjectPool<PacketWriter> _writerPool = new();

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
        using var packetWriter = new SpanWriter(stackalloc byte[1024], true);

        // Write OpCode
        packetWriter.Write(packet.OpCode);

        // Create a separate writer for the packet data
        //using var dataWriter = new PacketWriter();
        packet.Write(packetWriter);
        var packetData = packetWriter.Span;

        // Write the packet data
        packetWriter.Write(packetData);

        var array = packetWriter.ToSpan();


        // Return the serialized packet data
        return array.Span.ToArray();
    }

    /// <summary>
    /// Deserializes a byte array into a list of network packets.
    /// </summary>
    /// <param name="data">The byte array containing the packet data.</param>
    /// <returns>A list of deserialized packets, or an empty list if no valid packets could be parsed.</returns>
    public List<IUoNetworkPacket> ReadPackets(byte[] data)
    {
        List<IUoNetworkPacket> packets = [];
        var buffer = new Memory<byte>(data);

        while (buffer.Length > 0)
        {
            var packetResult = TryReadPacket(buffer);
            if (!packetResult.Success)
            {
                break;
            }

            packets.Add(packetResult.Packet);
            buffer = buffer[packetResult.ConsumedBytes..];
        }

        return packets;
    }

    /// <summary>
    /// Attempts to read a single packet from the buffer.
    /// </summary>
    /// <param name="buffer">The memory buffer containing packet data.</param>
    /// <returns>A result object containing the packet, consumed bytes, and success status.</returns>
    private PacketReadResult TryReadPacket(Memory<byte> buffer)
    {
        // Check if we have at least enough data for OpCode
        if (buffer.Length < 1)
        {
            _logger.LogWarning("Buffer too small for packet header");
            return PacketReadResult.Failed();
        }

        byte opCode = buffer.Span[0];
        if (!_packets.TryGetValue(opCode, out var packetFunc))
        {
            _logger.LogWarning("Packet with OpCode {OpCode} is not registered", opCode.ToString("X2"));
            return PacketReadResult.Failed();
        }

        IUoNetworkPacket packet = packetFunc();
        int expectedLength = DeterminePacketLength(packet, buffer);

        if (expectedLength < 0)
        {
            return PacketReadResult.Failed();
        }

        if (buffer.Length < expectedLength)
        {
            _logger.LogWarning(
                "Packet data truncated. Expected length: {Expected}, Actual available: {Actual}",
                expectedLength,
                buffer.Length
            );
            return PacketReadResult.Failed();
        }

        if (!TryParsePacket(packet, buffer[..expectedLength].ToArray(), expectedLength))
        {
            return PacketReadResult.Failed();
        }

        return new PacketReadResult(packet, expectedLength, true);
    }

    /// <summary>
    /// Determines the expected length of a packet based on its type and buffer content.
    /// </summary>
    /// <param name="packet">The packet to determine length for.</param>
    /// <param name="buffer">The buffer containing the packet data.</param>
    /// <returns>The expected length in bytes, or -1 if length cannot be determined.</returns>
    private int DeterminePacketLength(IUoNetworkPacket packet, Memory<byte> buffer)
    {
        int length = packet.Length;

        // Fixed length packet
        if (length != -1)
        {
            return length;
        }

        // Variable length packet - need at least 3 bytes (OpCode + Length field)
        if (buffer.Length < 3)
        {
            _logger.LogWarning("Buffer too small for variable length packet header: {Length} bytes", buffer.Length);
            return -1;
        }

        // Read the length from the packet (assuming it's at bytes 1-2)
        ushort packetLength = (ushort)((buffer.Span[1] << 8) | buffer.Span[2]);

        // Length field includes the entire packet length, no need to add OpCode bytes
        return packetLength;
    }

    /// <summary>
    /// Attempts to parse a packet from the provided data.
    /// </summary>
    /// <param name="packet">The packet instance to populate.</param>
    /// <param name="packetData">The raw packet data.</param>
    /// <param name="packetLength">The length of the packet data.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    private bool TryParsePacket(IUoNetworkPacket packet, byte[] packetData, int packetLength)
    {
        try
        {
            var packetReader = new SpanReader(packetData);


            // packetReader.Initialize(packetData, packetLength, true);

            // Read the packet content
            packetReader.ReadByte();
            packet.Read(packetReader);
            _logger.LogDebug("Successfully parsed packet: {PacketType}", packet.GetType().Name);

            // Return the packet to the pool
            //_readerPool.Return(packetReader);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing packet with OpCode {OpCode}", packetData[0].ToString("X2"));
            return false;
        }
    }
}
