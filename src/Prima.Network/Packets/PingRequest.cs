using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class PingRequest : BaseUoNetworkPacket
{

    public int Sequence { get; set; }

    public PingRequest() : base(0x73, 2)
    {

    }


    public override void Read(PacketReader reader)
    {
        Sequence = reader.ReadByte();
        base.Read(reader);
    }

    public override void Write(PacketWriter writer)
    {
        writer.WriteByte((byte)Sequence);
        base.Write(writer);
    }
}
