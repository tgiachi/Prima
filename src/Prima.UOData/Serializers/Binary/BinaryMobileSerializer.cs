using Prima.UOData.Entities;
using Prima.UOData.Extensions;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Serializers.Base;
using Prima.UOData.Types;

namespace Prima.UOData.Serializers.Binary;

public class BinaryMobileSerializer : BaseEntitySerializer<MobileEntity>
{
    public override void Serialize(BinaryWriter writer, MobileEntity entity, IPersistenceManager persistenceManager)
    {
        writer.Write(entity.Id);
        writer.Write(entity.Name);
        writer.Write(entity.IsPlayer);
        writer.Write(entity.Hue);
        writer.Write(entity.Position);
        writer.Write(entity.Direction);
        // Start to write items
        writer.Write(entity.Items.Count);

        foreach (var item in entity.Items)
        {
            writer.Write((byte)item.Key);
            writer.Write(item.Value.Id);
        }
    }

    public override object Deserialize(BinaryReader reader, IPersistenceManager persistenceManager)
    {
        var mobile = new MobileEntity
        {
            Id = reader.ReadSerial(),
            Name = reader.ReadString(),
            IsPlayer = reader.ReadBoolean(),
            Hue = reader.ReadInt32(),
            Position = reader.ReadPoint3D(),
            Direction = reader.ReadDirection()
        };

        // Read items count
        var itemsCount = reader.ReadInt32();

        for (var i = 0; i < itemsCount; i++)
        {
            var itemId = reader.ReadSerial();
            var layer = (Layer)reader.ReadByte();
            var item = new ItemEntity(itemId);
            mobile.Items.Add(layer, item);
        }

        return mobile;
    }
}
