using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities;
using Prima.UOData.Id;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class DrawGamePlayer : BaseUoNetworkPacket
{
    public Serial MobileId { get; set; }

    public short BodyType { get; set; }

    public int Hue { get; set; }

    public byte StatusFlag { get; set; } = 0x00;

    public Point3D Position { get; set; }

    public Direction Direction { get; set; }

    public DrawGamePlayer(MobileEntity mobile) : this()
    {
        MobileId = mobile.Id;
        BodyType = 0x190;
        Position = mobile.Position;
        Direction = mobile.Direction;
        Hue = mobile.Hue;
    }

    public DrawGamePlayer() : base(0x20, 19)
    {
    }

    public override Span<byte> Write()
    {
        using var writer = new SpanWriter(stackalloc byte[Length - 1]);

        writer.Write((uint)MobileId);
        writer.Write((short)BodyType);
        writer.Write((byte)0);
        writer.Write((short)Hue);
        writer.Write((byte)StatusFlag);
        writer.Write((short)Position.X);
        writer.Write((short)Position.Y);
        writer.Write((short)0);
        writer.Write((byte)Direction);
        writer.Write((short)Position.Z);

        return writer.Span.ToArray();
    }
}
