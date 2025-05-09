using Prima.Network.Packets.Base;
using Prima.Network.Serializers;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class CharacterCreation : BaseUoNetworkPacket
{
    public string Name { get; set; }

    public int LoginCount { get; set; }
    public CharacterCreateFlags ClientFlags { get; set; }

    public CharacterCreation() : base(0xF8, 106)
    {
    }

    public override void Read(PacketReader reader)
    {
        reader.ReadInt16(); // (0xedededed)
        reader.ReadInt16(); // (0xffffffff)
        reader.ReadByte(); //(0xffffffff)

        Name = reader.ReadFixedString(30);

        reader.ReadByte();
        reader.ReadByte();

        ClientFlags = (CharacterCreateFlags)reader.ReadInt32();

        reader.ReadInt32();

        LoginCount = reader.ReadInt32();

    }
}
