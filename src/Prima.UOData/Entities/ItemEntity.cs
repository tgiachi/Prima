using Prima.Core.Server.Attributes;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities.Base;


namespace Prima.UOData.Entities;

[SerializableHeader(0x02, "items")]
public class ItemEntity : BaseWorldEntity
{
    public string Name { get; set; }

    public int Hue { get; set; }

    public Point3D Position { get; set; }
}
