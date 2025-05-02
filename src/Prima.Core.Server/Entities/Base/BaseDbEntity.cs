using LiteDB;
using Prima.Core.Server.Interfaces.Entities;

namespace Prima.Core.Server.Entities.Base;

public abstract class BaseDbEntity : IPrimaDbEntity
{
    public ObjectId Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
