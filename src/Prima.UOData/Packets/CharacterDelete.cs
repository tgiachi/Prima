using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;

namespace Prima.UOData.Packets;

public class CharacterDelete : BaseUoNetworkPacket
{
    public int Slot { get; set; }

    public CharacterDelete() : base(0x83, 39)
    {
    }


    public override void Read(SpanReader reader)
    {
        reader.ReadBytes(30);
        Slot = reader.ReadInt32();

        base.Read(reader);
    }
}
