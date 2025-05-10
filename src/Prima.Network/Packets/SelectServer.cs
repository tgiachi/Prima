using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;


namespace Prima.Network.Packets;

/// <summary>
/// Represents a server selection packet sent from the client to the login server
/// when the player selects a game server from the list.
/// </summary>
/// <remarks>
/// OpCode: 0xA0
/// </remarks>
public class SelectServer() : BaseUoNetworkPacket(0xA0, 3)
{
    /// <summary>
    /// Gets or sets the ID of the selected shard (game server).
    /// This should match the Index of a GameServerEntry.
    /// </summary>
    public ushort ShardId { get; set; }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public override void Read(SpanReader reader)
    {
        ShardId = reader.ReadUInt16();
    }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write data to.</param>
    public override void Write(SpanWriter writer)
    {
        writer.Write(ShardId);
    }
}
