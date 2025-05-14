using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class CharacterLogin : BaseUoNetworkPacket
{
    public string Name { get; set; }

    public CharacterCreateFlags ClientFlags { get; set; }

    public int LoginCount { get; set; }

    public int Slot { get; set; }


    public CharacterLogin() : base(0x5D, 73)
    {
    }

    public override void Read(SpanReader reader)
    {
        reader.ReadInt32(); // (0xedededed)

        Name = reader.ReadAscii(30);

        reader.ReadBytes(2);

        ClientFlags = (CharacterCreateFlags)reader.ReadInt32();

        reader.ReadInt32();

        LoginCount = reader.ReadInt32();

        reader.ReadBytes(16);

        Slot = reader.ReadInt32();
    }
}
