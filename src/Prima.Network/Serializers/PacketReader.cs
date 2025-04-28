using System.Text;

namespace Prima.Network.Serializers;

public class PacketReader : BinaryReader
{
    public PacketReader(byte[] data) : base(new MemoryStream(data))
    {
    }

    public ushort ReadUInt16BE()
    {
        var bytes = base.ReadBytes(2);
        Array.Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    public uint ReadUInt32BE()
    {
        var bytes = base.ReadBytes(4);
        Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public string ReadFixedString(int length)
    {
        var bytes = base.ReadBytes(length);
        return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
    }

    public string ReadString()
    {
        ushort length = ReadUInt16BE();
        byte[] bytes = base.ReadBytes(length);
        return Encoding.ASCII.GetString(bytes);
    }

    public T ReadEnum<T>() where T : Enum
    {
        byte value = ReadByte();
        return (T)Enum.ToObject(typeof(T), value);
    }
}
