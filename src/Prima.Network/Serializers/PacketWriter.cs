using System.Text;

namespace Prima.Network.Serializers;

public sealed class PacketWriter : BinaryWriter
{
    private readonly MemoryStream _stream;

    public PacketWriter() : base(new MemoryStream()) => _stream = (MemoryStream)BaseStream;


    public void WriteUInt16BE(ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        Write(bytes);
    }

    public void WriteUInt32BE(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        Write(bytes);
    }

    public void WriteFixedString(string text, int length)
    {
        if (text.Length > length)
        {
            text = text[..length];
        }

        var paddedText = text.PadRight(length, '\0');

        var bytes = Encoding.ASCII.GetBytes(paddedText);

        Write(bytes);
    }

    public void WriteByte(byte value)
    {
        Write(value);
    }

    public void WriteString(string text)
    {
        var bytes = Encoding.ASCII.GetBytes(text);
        WriteUInt16BE((ushort)bytes.Length);
        Write(bytes);
    }

    public void WriteEnum<T>(T value) where T : Enum
    {
        byte byteValue = Convert.ToByte(value);
        Write(byteValue);
    }

    public byte[] ToArray()
    {
        return _stream.ToArray();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stream?.Dispose();
        }

        base.Dispose(disposing);
    }
}
