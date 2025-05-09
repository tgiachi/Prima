namespace Prima.UOData.Types;

[Flags]
public enum CharacterCreateFlags : int
{
    T2A = 0x00,
    Renaissance = 0x01,
    ThirdDawn = 0x02,
    LBR = 0x04,
    AOS = 0x08,
    SE = 0x10,
    SA = 0x20,
    UO3D = 0x40,
    Reserved = 0x80,
    ThreeDClient = 0x100
}
