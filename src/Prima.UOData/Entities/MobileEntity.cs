using Prima.Core.Server.Attributes;
using Prima.UOData.Entities.Base;

namespace Prima.UOData.Entities;

[SerializableHeader(0x01, "mobiles")]
public class MobileEntity : BaseWorldEntity
{
    public bool IsPlayer { get; set; }


}
