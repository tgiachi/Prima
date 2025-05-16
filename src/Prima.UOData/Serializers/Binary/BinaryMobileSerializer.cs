using Prima.UOData.Entities;
using Prima.UOData.Extensions;
using Prima.UOData.Serializers.Base;
using Prima.UOData.Types;

namespace Prima.UOData.Serializers.Binary;

public class BinaryMobileSerializer : BaseEntitySerializer<MobileEntity>
{
    public override object Deserialize(byte[] data)
    {
        using var buffer = new MemoryStream(data);
        using var stream = new BinaryReader(buffer);

        var mobile = new MobileEntity
        {
            Id = stream.ReadSerial(),
            Name = stream.ReadString(),
            IsPlayer = stream.ReadBoolean(),
            Hue = stream.ReadInt32(),
            Position = stream.ReadPoint3D(),
            Direction = stream.ReadDirection()
        };

        // Read items count
        var itemsCount = stream.ReadInt32();

        for (var i = 0; i < itemsCount; i++)
        {
            var itemId = stream.ReadSerial();
            var layer = (Layer)stream.ReadByte();
            var item = new ItemEntity(itemId);
            mobile.Items.Add(layer, item);
        }

        return mobile;
    }

    public override byte[] Serialize(MobileEntity entity)
    {
        using var buffer = new MemoryStream();
        using var stream = new BinaryWriter(buffer);

        stream.Write(entity.Id);
        stream.Write(entity.Name);
        stream.Write(entity.IsPlayer);
        stream.Write(entity.Hue);
        stream.Write(entity.Position);
        stream.Write(entity.Direction);
        // Start to write items
        stream.Write(entity.Items.Count);

        foreach (var item in entity.Items)
        {
            stream.Write((byte)item.Key);
            stream.Write(item.Value.Id);
        }


        return buffer.ToArray();
    }
}
