using System.Text;
using Orion.Foundations.Spans;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Entities;
using Prima.UOData.Types;

namespace Prima.UOData.Extensions;

public static class StreamWriterExtension
{
    public static void Write(this BinaryWriter writer, string? str)
    {
        if (str == null)
        {
            writer.Write((byte)0);
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(str);
        writer.Write((byte)bytes.Length);
        writer.Write(bytes);
    }


    public static void Write(this BinaryWriter writer, Point2D point)
    {
        writer.Write((short)point.X);
        writer.Write((short)point.Y);
    }

    public static Point2D ReadPoint(this BinaryReader reader)
    {
        var x = reader.ReadInt16();
        var y = reader.ReadInt16();

        return new Point2D(x, y);
    }

    public static Point3D ReadPoint3D(this BinaryReader reader)
    {
        var x = reader.ReadInt16();
        var y = reader.ReadInt16();
        var z = reader.ReadInt16();

        return new Point3D(x, y, z);
    }

    public static void Write(this BinaryWriter writer, Point3D point)
    {
        writer.Write((short)point.X);
        writer.Write((short)point.Y);
        writer.Write((short)point.Z);
    }

    public static void Write(this BinaryWriter writer, Direction direction)
    {
        writer.Write((byte)direction);
    }


    public static void Write(this BinaryWriter writer, Serial serial)
    {
        writer.Write((uint)serial.Value);
    }


    public static Serial ReadSerial(this BinaryReader reader)
    {
        return new Serial(reader.ReadUInt32());
    }

    public static Serial ReadSerial(this SpanReader reader)
    {
        return new Serial(reader.ReadUInt32());
    }


    public static Direction ReadDirection(this BinaryReader reader)
    {
        return (Direction)reader.ReadByte();
    }


    public static void Write(this BinaryWriter writer, IHaveSerial serial)
    {
        writer.Write(serial.Id.Value);
    }

    public static string ReadString(this BinaryReader reader)
    {
        var length = reader.ReadByte();
        if (length == 0)
            return string.Empty;

        var bytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }


    public static void Write<T>(this BinaryWriter writer, List<T>? list) where T : class, IHaveSerial
    {
        if (list == null)
        {
            writer.Write((byte)0);
            return;
        }

        writer.Write((byte)list.Count);

        foreach (var item in list)
        {
            writer.Write(item);
        }
    }
}
