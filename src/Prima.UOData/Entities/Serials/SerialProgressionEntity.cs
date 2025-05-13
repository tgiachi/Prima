using Prima.Core.Server.Entities.Base;

namespace Prima.UOData.Entities.Serials;

public class SerialProgressionEntity : BaseDbEntity
{
    public string SerialType { get; set; }

    public int LastSerial { get; set; }
}
