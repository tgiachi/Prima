using Orion.Foundations.Spans;
using Prima.UOData.Entities;
using Prima.UOData.Extensions;
using Prima.UOData.Serializers.Base;

namespace Prima.UOData.Serializers.Binary;

public class BinaryItemSerializer : BaseEntitySerializer<ItemEntity>
{
    public override object Deserialize(byte[] data)
    {
        using var stream = new SpanReader(data);

        return new ItemEntity
        {
            Id = stream.ReadSerial(),
            Name = stream.ReadAscii(),
            Hue = stream.ReadUInt16(),
            Position = stream.ReadPoint3D()
        };
    }

    public override byte[] Serialize(ItemEntity entity)
    {
        using var stream = new SpanWriter(1, true);

        stream.Write(entity.Id);
        stream.WriteAscii(entity.Name);
        stream.Write(entity.Hue);
        stream.Write(entity.Position);

        return stream.ToSpan().Span.ToArray();
    }
}
