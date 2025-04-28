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
        var bytes = new byte[length];
        var stringBytes = Encoding.ASCII.GetBytes(text);
        Array.Copy(stringBytes, bytes, Math.Min(length, stringBytes.Length));
        Write(bytes);
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stream?.Dispose();
        }

        base.Dispose(disposing);
    }
}
