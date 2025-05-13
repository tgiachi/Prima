using Prima.UOData.Id;

namespace Prima.UOData.Interfaces.Persistence.Entities;

public interface ISerializableEntity
{
    Serial Id { get; set; }
}
