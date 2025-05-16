namespace Prima.UOData.Types;

public enum CharacterStatus : byte
{
    Normal = 0x00,
    Unknown = 0x01,
    CanAlterPaperdoll = 0x02,
    Poisoned = 0x04,
    GoldenHealth = 0x08,
    Unknown0x10 = 0x10,
    Unknown0x20 = 0x20,
    WarMode = 0x40
}
