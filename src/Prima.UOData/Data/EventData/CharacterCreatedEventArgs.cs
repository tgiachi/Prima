using Prima.UOData.Data.Map;
using Prima.UOData.Types;

namespace Prima.UOData.Data.EventData;

public class CharacterCreatedEventArgs(
    // NetState state, IAccount a,
    string name,
    bool female,
    int hue,
    int inte,
    int str,
    int dex,
    CityInfo city,
    Dictionary<SkillName, int> skills,
    int shirtHue,
    int pantsHue,
    int hairId,
    int hairHue,
    int beardId,
    int beardHue,
    ProfessionInfo profession,
    Race race
)
{
    // public NetState State { get; } = state;
    //
    // public IAccount Account { get; } = a;
    //
    // public Mobile Mobile { get; set; }

    public string Name { get; } = name;

    public bool Female { get; } = female;

    public int Hue { get; } = hue;

    public int Inte { get; } = inte;
    public int Str { get; } = str;
    public int Dex { get; } = dex;

    public CityInfo City { get; } = city;

    public Dictionary<SkillName, int> Skills { get; } = skills;

    public int ShirtHue { get; } = shirtHue;

    public int PantsHue { get; } = pantsHue;

    public int HairID { get; } = hairId;

    public int HairHue { get; } = hairHue;

    public int BeardID { get; } = beardId;

    public int BeardHue { get; } = beardHue;

    public ProfessionInfo Profession { get; set; } = profession;

    public Race Race { get; } = race;
}
