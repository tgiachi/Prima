using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class SelectServer() : BaseUoNetworkPacket(0xA0)
{
    public byte Command { get; set; }

    public ushort ShardId { get; set; }

    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();
        ShardId = reader.ReadUInt16BE();
    }

    public override void Write(PacketWriter writer)
    {
        writer.WriteByte(Command);
        writer.WriteUInt16BE(ShardId);
    }
}
