using LiteDB;

namespace Prima.Core.Server.Interfaces.Entities;

public interface IPrimaDbEntity
{
    ObjectId Id { get; set; }

    DateTime Created { get; set; }

    DateTime Updated { get; set; }

}
