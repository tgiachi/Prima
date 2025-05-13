using System.Text;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Id;

namespace Prima.UOData.Extensions;

public static class BinaryWriterExtension
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

    public static void Write(this BinaryWriter writer, Point3D point)
    {
        writer.Write((short)point.X);
        writer.Write((short)point.Y);
        writer.Write((short)point.Z);
    }

    public static void Write(this BinaryWriter writer, Serial serial)
    {
        writer.Write(serial.Value);
    }
    


}
