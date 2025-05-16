using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;

namespace Prima.UOData.Packets;

/// <summary>
/// Represents a General Information packet (0xBF) used for various extended functionality in the UO protocol.
/// This packet serves as a container for numerous different sub-packets identified by a subcommand.
/// </summary>
/// <remarks>
/// OpCode: 0xBF
/// The General Information packet has a variable length and can contain different data
/// based on the subcommand value. It's one of the most versatile packets in the UO protocol
/// and is used for features added after the initial protocol design.
/// </remarks>
public class GeneralInfoPacket : BaseUoNetworkPacket
{
    /// <summary>
    /// Gets or sets the subcommand that identifies the specific type of information being sent.
    /// </summary>
    public ushort Subcommand { get; set; }

    /// <summary>
    /// Gets or sets the raw payload data for this packet.
    /// The content varies based on the subcommand.
    /// </summary>
    public byte[] Payload { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the GeneralInfoPacket class.
    /// </summary>
    public GeneralInfoPacket() : base(0xBF, -1) // Variable length packet
    {
    }

    /// <summary>
    /// Initializes a new instance of the GeneralInfoPacket class with the specified subcommand and payload.
    /// </summary>
    /// <param name="subcommand">The subcommand value that identifies the specific type of information.</param>
    /// <param name="payload">The raw payload data for this packet.</param>
    public GeneralInfoPacket(ushort subcommand, byte[] payload) : this()
    {
        Subcommand = subcommand;
        Payload = payload ?? Array.Empty<byte>();
    }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader containing the packet data.</param>
    public override void Read(SpanReader reader)
    {
        // Skip the first 3 bytes (already read: opcode and packet length)
        // if this causes issues, you may need to not skip them depending on how your reader is implemented

        // Read the subcommand
        Subcommand = reader.ReadUInt16();

        // Read remaining bytes as payload
        var remainingLength = reader.Remaining;
        if (remainingLength > 0)
        {
            Payload = new byte[remainingLength];
            Payload = reader.ReadBytes(remainingLength);
        }
        else
        {
            Payload = [];
        }
    }

    /// <summary>
    /// Writes the packet data to a byte span.
    /// </summary>
    /// <returns>A span containing the serialized packet data.</returns>
    public override Span<byte> Write()
    {
        // Calculate the total packet length
        // 5 = 1 (opcode) + 2 (length) + 2 (subcommand)
        int packetLength = 5 + Payload.Length;

        // Create a writer with the calculated capacity
        using var writer = new SpanWriter(stackalloc byte[packetLength], true);

        // Write packet length (big endian)
        writer.Write((ushort)packetLength);

        // Write subcommand
        writer.Write(Subcommand);

        // Write payload
        if (Payload.Length > 0)
        {
            writer.Write(Payload);
        }

        return writer.ToSpan().Span;
    }

    /// <summary>
    /// Returns a string representation of this packet for debugging purposes.
    /// </summary>
    /// <returns>A string containing the packet type, OpCode, subcommand, and payload length.</returns>
    public override string ToString()
    {
        return base.ToString() + $" {{ Subcommand: 0x{Subcommand:X4}, PayloadLength: {Payload.Length} }}";
    }
}
