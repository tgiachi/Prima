using LiteDB;
using Prima.Core.Server.Entities.Base;
using Prima.UOData.Id;

namespace Prima.UOData.Entities.Db;

public class CharacterEntity : BaseDbEntity
{

    public ObjectId AccountId { get; set; }

    public int Slot { get; set; }

    public Serial MobileId { get; set; }
}
