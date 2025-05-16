using Orion.Foundations.Spans;

namespace Prima.UOData.Packets;

/// <summary>
/// Base class for specialized sub-packets of the 0xBF General Information packet.
/// </summary>
public abstract class GeneralInfoSubPacket
{
    /// <summary>
    /// Gets the subcommand value that identifies this specific type of information.
    /// </summary>
    public abstract ushort Subcommand { get; }

    /// <summary>
    /// Serializes the sub-packet data to a byte array.
    /// </summary>
    /// <returns>A byte array containing the serialized sub-packet data.</returns>
    public abstract byte[] Serialize();

    /// <summary>
    /// Deserializes the sub-packet data from a packet reader.
    /// </summary>
    /// <param name="reader">The packet reader containing the sub-packet data.</param>
    public abstract void Deserialize(SpanReader reader);

    /// <summary>
    /// Creates a General Information packet containing this sub-packet's data.
    /// </summary>
    /// <returns>A GeneralInfoPacket instance ready to be sent.</returns>
    public GeneralInfoPacket CreatePacket()
    {
        return new GeneralInfoPacket(Subcommand, Serialize());
    }
}
