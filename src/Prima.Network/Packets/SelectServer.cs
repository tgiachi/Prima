using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

/// <summary>
/// Represents a server selection packet sent from the client to the login server
/// when the player selects a game server from the list.
/// </summary>
/// <remarks>
/// OpCode: 0xA0
/// </remarks>
public class SelectServer() : BaseUoNetworkPacket(0xA0)
{
    /// <summary>
    /// Gets or sets the command byte for this packet.
    /// </summary>
    public byte Command { get; set; }

    /// <summary>
    /// Gets or sets the ID of the selected shard (game server).
    /// This should match the Index of a GameServerEntry.
    /// </summary>
    public ushort ShardId { get; set; }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();
        ShardId = reader.ReadUInt16BE();
    }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write data to.</param>
    public override void Write(PacketWriter writer)
    {
        writer.WriteByte(Command);
        writer.WriteUInt16BE(ShardId);
    }
}
