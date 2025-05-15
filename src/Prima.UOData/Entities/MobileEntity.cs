using MessagePack;
using Prima.Core.Server.Attributes;
using Prima.Core.Server.Types;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities.Base;
using Prima.UOData.Id;
using Prima.UOData.Types;

namespace Prima.UOData.Entities;

[SerializableHeader(0x01)]
public class MobileEntity : BaseWorldEntity
{
    public string Name { get; set; }

    public bool IsPlayer { get; set; }

    public CommandPermissionType AccessLevel { get; set; }

    public int Hue { get; set; }

    public Point3D Position { get; set; }

    public Direction Direction { get; set; }

    public MobileEntity()
    {
    }

    public MobileEntity(Serial serial)
    {
        Id = serial;
    }
}
