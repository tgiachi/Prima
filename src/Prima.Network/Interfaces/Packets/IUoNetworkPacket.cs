using Prima.Network.Serializers;

namespace Prima.Network.Interfaces.Packets;

/// <summary>
/// Defines the interface for all Ultima Online network packets.
/// All packet classes must implement this interface.
/// </summary>
public interface IUoNetworkPacket
{
    /// <summary>
    /// Gets the operation code that identifies the packet type.
    /// Each packet type has a unique OpCode in the UO protocol.
    /// </summary>
    byte OpCode { get; }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader containing the packet data.</param>
    void Read(PacketReader reader);

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write packet data to.</param>
    void Write(PacketWriter writer);
}
