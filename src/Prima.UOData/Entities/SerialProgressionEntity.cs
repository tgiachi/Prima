using Prima.Core.Server.Entities.Base;

namespace Prima.UOData.Entities;

public class SerialProgressionEntity : BaseDbEntity
{
    public string SerialType { get; set; }

    public int LastSerial { get; set; }
}
