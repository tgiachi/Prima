using MessagePack;
using Prima.Core.Server.Attributes;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Types;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities.Base;
using Prima.UOData.Id;
using Prima.UOData.Types;

namespace Prima.UOData.Entities;

[SerializableHeader(0x01)]
public class MobileEntity : BaseWorldEntity
{
    public Serial ModelId { get; set; }
    public string Name { get; set; }

    public string Title { get; set; }

    public NetworkSession? NetworkSession { get; set; }
    public bool IsPlayer { get; set; }

    public bool IsRunning { get; set; }

    public CommandPermissionType AccessLevel { get; set; }

    public int Hue { get; set; }

    public Point3D Position { get; set; }
    public Direction Direction { get; set; }

    public Dictionary<Layer, List<ItemEntity>> Items { get; set; } = new();

    public MobileEntity()
    {
    }

    public void MoveForward(int distance)
    {
        var x = Position.X;
        var y = Position.Y;

        switch (Direction)
        {
            case Direction.North:
                y -= distance;
                break;
            case Direction.East:
                x += distance;
                break;
            case Direction.South:
                y += distance;
                break;
            case Direction.West:
                x -= distance;
                break;
        }

        Position = new Point3D(x, y, Position.Z);
    }

    public MobileEntity(Serial serial)
    {
        Id = serial;
    }
}
