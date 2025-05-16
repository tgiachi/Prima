using Orion.Foundations.Spans;
using Prima.UOData.Entities;
using Prima.UOData.Extensions;
using Prima.UOData.Serializers.Base;

namespace Prima.UOData.Serializers.Binary;

public class BinaryItemSerializer : BaseEntitySerializer<ItemEntity>
{
    public override object Deserialize(byte[] data)
    {
        using var stream = new BinaryReader(new MemoryStream(data));

        return new ItemEntity
        {
            Id = stream.ReadSerial(),
            Name = stream.ReadString(),
            Hue = stream.ReadInt32(),
            Position = stream.ReadPoint3D()
        };
    }

    public override byte[] Serialize(ItemEntity entity)
    {
        var buffer = new MemoryStream();
        using var stream = new BinaryWriter(buffer);

        stream.Write(entity.Id);
        stream.Write(entity.Name);
        stream.Write(entity.Hue);
        stream.Write(entity.Position);

        return buffer.ToArray();
    }

}
