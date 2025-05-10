using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;


namespace Prima.Network.Packets;

public class PingRequest : BaseUoNetworkPacket
{

    public int Sequence { get; set; }

    public PingRequest() : base(0x73, 2)
    {

    }


    public override void Read(SpanReader reader)
    {
        Sequence = reader.ReadByte();
        base.Read(reader);
    }

    public override void Write(SpanWriter writer)
    {
        writer.Write((byte)Sequence);
        base.Write(writer);
    }
}
