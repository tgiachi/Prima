using Orion.Foundations.Spans;
using Prima.UOData.Entities;
using Prima.UOData.Extensions;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Serializers.Base;

namespace Prima.UOData.Serializers.Binary;

public class BinaryItemSerializer : BaseEntitySerializer<ItemEntity>
{
    public override void Serialize(BinaryWriter writer, ItemEntity entity, IPersistenceManager persistenceManager)
    {
        writer.Write(entity.Id);
        writer.Write(entity.Name);
        writer.Write(entity.Hue);
        writer.Write(entity.Position);
    }

    public override object Deserialize(BinaryReader reader, IPersistenceManager persistenceManager)
    {
        return new ItemEntity
        {
            Id = reader.ReadSerial(),
            Name = reader.ReadString(),
            Hue = reader.ReadInt32(),
            Position = reader.ReadPoint3D()
        };
    }
}
