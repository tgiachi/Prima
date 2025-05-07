namespace Prima.UOData.Data.Skills;

public sealed class SkillInfo
{
    public int Index { get; set; }
    public bool IsAction { get; set; }

    public string Name { get; set; }

    public int Extra { get; private set; }

    public SkillInfo(int nr, string name, bool action, int extra)
    {
        Index = nr;
        Name = name;
        IsAction = action;
        Extra = extra;
    }

    public override string ToString()
    {
        return $"{Index} - {Name} ({(IsAction ? "Action" : "Non-Action")})";
    }
}
