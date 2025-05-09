using System.Runtime.CompilerServices;
using Orion.Foundations.Extensions;
using Prima.UOData.Data.Skills;
using Prima.UOData.Types;

namespace Prima.UOData.Data;

public class ProfessionInfo
{

    public static ProfessionInfo[] Professions;

    public int ID { get; set; }
    public string Name { get; set; }
    public int NameID { get; set; }
    public int DescID { get; set; }
    public bool TopLevel { get; set; }
    public int GumpID { get; set; }

    public (SkillName, byte)[] Skills => _skills;

    private (SkillName, byte)[] _skills;

    public byte[] Stats { get; }

    public ProfessionInfo()
    {
        Name = string.Empty;

        _skills = new (SkillName, byte)[4];
        Stats = new byte[3];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool VerifyProfession(int profIndex) => profIndex > 0 && profIndex < Professions.Length;

    public static bool GetProfession(int profIndex, out ProfessionInfo profession)
    {
        if (!VerifyProfession(profIndex))
        {
            profession = null;
            return false;
        }

        return (profession = Professions[profIndex]) != null;
    }

    public static bool TryGetSkillName(string name, out SkillName skillName)
    {
        if (Enum.TryParse(name, out skillName))
        {
            return true;
        }

        var lowerName = name?.ToLowerInvariant().RemoveOrdinal(" ");

        if (!string.IsNullOrEmpty(lowerName))
        {
            foreach (var so in SkillInfo.Table)
            {
                if (lowerName == so.ProfessionSkillName.ToLowerInvariant())
                {
                    skillName = (SkillName)so.SkillID;
                    return true;
                }
            }
        }

        return false;
    }



    public void FixSkills()
    {
        var index = _skills.Length - 1;
        while (index >= 0)
        {
            var skill = _skills[index];
            if (skill is not (SkillName.Alchemy, 0))
            {
                break;
            }

            index--;
        }

        Array.Resize(ref _skills, index + 1);
    }

}
