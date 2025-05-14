namespace Prima.UOData.Types;


[Flags]
public enum Direction : byte
{
    North = 0x0,
    Right = 0x1,
    East = 0x2,
    Down = 0x3,
    South = 0x4,
    Left = 0x5,
    West = 0x6,
    Up = 0x7,

    Mask = 0x7,
    Running = 0x80,
    ValueMask = 0x87
}
