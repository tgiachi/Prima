using Prima.Core.Server.Attributes;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities.Base;
using Prima.UOData.Types;

namespace Prima.UOData.Entities;

[SerializableHeader(0x01, "mobiles")]
public class MobileEntity : BaseWorldEntity
{
    public string Name { get; set; }
    public bool IsPlayer { get; set; }
    public int Hue { get; set; }
    public Point3D Position { get; set; }

    public Direction Direction { get; set; }


}
