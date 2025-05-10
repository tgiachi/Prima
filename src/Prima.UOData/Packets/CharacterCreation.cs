using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;

using Prima.UOData.Data;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class CharacterCreation : BaseUoNetworkPacket
{
    public string Name { get; set; }

    public int Slot { get; set; }

    public int LoginCount { get; set; }
    public CharacterCreateFlags ClientFlags { get; set; }

    public Dictionary<SkillName, int> Skills { get; set; } = new();

    public ProfessionInfo Profession { get; set; } = new();

    public SexType Sex { get; set; }

    public int Str { get; set; }

    public int Dex { get; set; }

    public int Int { get; set; }

    public short HairStyle { get; set; }

    public short HairColor { get; set; }

    public short FacialHair { get; set; }

    public short FacialHairColor { get; set; }

    public short StartingLocation { get; set; }


    public short ShirtColor { get; set; }

    public short PantsColor { get; set; }

    public CharacterCreation() : base(0xF8, 106)
    {
    }

    public override void Read(SpanReader reader)
    {
        reader.ReadInt32(); // (0xedededed)
        reader.ReadInt32(); // (0xffffffff)
        reader.ReadByte();  //(0xffffffff)

        Name = reader.ReadAscii(30);

        reader.ReadByte();
        reader.ReadByte();

        ClientFlags = (CharacterCreateFlags)reader.ReadInt32();

        reader.ReadInt32();

        LoginCount = reader.ReadInt32();
        Profession = ProfessionInfo.Professions[reader.ReadByte()];

        reader.Read(new byte[15]);

        Sex = (SexType)reader.ReadByte();

        Str = reader.ReadByte();
        Dex = reader.ReadByte();
        Int = reader.ReadByte();

        for (var i = 0; i < 4; i++)
        {
            var skillName = (SkillName)reader.ReadByte();
            var skillValue = reader.ReadByte();

            Skills.Add(skillName, skillValue);
        }

        HairStyle = reader.ReadInt16();
        HairColor = reader.ReadInt16();

        FacialHair = reader.ReadInt16();
        FacialHairColor = reader.ReadInt16();

        StartingLocation = reader.ReadInt16();

        reader.ReadInt16(); // (0x00000000)

        Slot = reader.ReadInt16();

        reader.ReadInt32();

        ShirtColor = reader.ReadInt16();

        PantsColor = reader.ReadInt16();
    }
}
