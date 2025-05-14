using Prima.Core.Server.Attributes;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities.Base;
using Prima.UOData.Id;


namespace Prima.UOData.Entities;

[SerializableHeader(0x02)]
public class ItemEntity : BaseWorldEntity
{
    public string Name { get; set; }
    public int Hue { get; set; }

    public Point3D Position { get; set; }

    public int Amount { get; set; }

    public ItemEntity()
    {
    }

    public ItemEntity(Serial serial)
    {
        Id = serial;
    }
}
