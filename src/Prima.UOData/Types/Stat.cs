namespace Prima.UOData.Types;

public enum Stat
{
    Str,
    Dex,
    Int
}


[Flags]
public enum StatType : byte
{
    Str = 1,
    Dex = 2,
    Int = 4,
    All = 7
}
