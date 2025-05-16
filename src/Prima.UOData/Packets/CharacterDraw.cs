using System.Runtime.CompilerServices;
using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities;
using Prima.UOData.Extensions;
using Prima.UOData.Id;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class CharacterDraw : BaseUoNetworkPacket
{
    public Serial Serial { get; set; }

    public Serial ModelId { get; set; }

    public Point3D Position { get; set; }

    public Direction Direction { get; set; }

    public short Hue { get; set; }

    public Notoriety Notoriety { get; set; }

    public CharacterStatus StatusFlag { get; set; }

    public int ItemCount { get; set; } = 0;

    public readonly Dictionary<Layer, ItemEntity> Items = new();

    public CharacterDraw(MobileEntity mobile) : this()
    {
        Serial = mobile.Id;
        ModelId = mobile.ModelId;
        Position = mobile.Position;
        Direction = mobile.Direction;
        StatusFlag = mobile.StatusFlag;
        Hue = (short)mobile.Hue;
        Notoriety = mobile.Notoriety;
        StatusFlag = 0x00;
        ItemCount = mobile.Items.Count;

        foreach (var item in mobile.Items)
        {
            Items.Add(item.Key, item.Value);
        }
    }

    public CharacterDraw() : base(0x78, -1)
    {
    }

    public override Span<byte> Write()
    {
        using var writer = new SpanWriter(stackalloc byte[10], true);

        writer.Write(Serial);
        writer.Write(ModelId);
        writer.Write((short)Position.X);
        writer.Write((short)Position.Y);
        writer.Write((short)Position.Z);
        writer.Write((byte)Direction);
        writer.Write(Hue);
        writer.Write((byte)StatusFlag);
        writer.Write((byte)Notoriety);

        if (ItemCount == 0)
        {
            writer.Write((int)0);
            writer.Write((byte)0);
        }
        else
        {
            writer.Write(ItemCount);

            foreach (var (layer, item) in Items)
            {
                writer.Write(item.Id);

                var modelId = item.ModelId & 0x7FFF;
                var writeHue = item.Hue != 0;

                if (writeHue)
                {
                    modelId |= 0x8000;
                }

                writer.Write(modelId);
                writer.Write((byte)layer);

                if (writeHue)
                {
                    writer.Write((short)item.Hue);
                }
            }

            writer.Write(0);
        }


        return writer.Span.ToArray();
    }
}
