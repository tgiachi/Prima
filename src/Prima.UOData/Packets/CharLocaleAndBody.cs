using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities;
using Prima.UOData.Id;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class CharLocaleAndBody : BaseUoNetworkPacket
{
    public Serial MobileId { get; set; }
    public short BodyType { get; set; }
    public Point3D Position { get; set; }
    public Direction Direction { get; set; }

    public Point2D MapSize { get; set; }


    public CharLocaleAndBody(MobileEntity mobile) : this()
    {
        MobileId = mobile.Id;
        BodyType = 0x190;
        Position = mobile.Position;
        Direction = mobile.Direction;
        MapSize = new Point2D(7168, 4096);
    }

    public CharLocaleAndBody() : base(0x1B, 37)
    {
    }

    public override Span<byte> Write()
    {
        using var packetWriter = new SpanWriter(stackalloc byte[36], true);

        packetWriter.Write((uint)MobileId);
        packetWriter.Write((int)0);
        packetWriter.Write((short)BodyType);
        packetWriter.Write((short)Position.X);
        packetWriter.Write((short)Position.Y);
        packetWriter.Write((byte)0);
        packetWriter.Write((short)Position.Z);
        packetWriter.Write((byte)Direction);
        packetWriter.Write((int)0);
        packetWriter.Write((int)0);
        packetWriter.Write((byte)0);
        packetWriter.Write((short)MapSize.X);
        packetWriter.Write((short)MapSize.Y);
        packetWriter.Write((short)0);
        packetWriter.Write((int)0);


        return packetWriter.Span.ToArray();
    }
}
