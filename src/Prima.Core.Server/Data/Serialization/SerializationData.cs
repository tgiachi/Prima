namespace Prima.Core.Server.Data.Serialization;

public class SerializationData
{
    public byte Header { get; set; }

    public long Length { get; set; }

    public byte[] Data { get; set; }

    public SerializationData(byte header, long length, byte[] data)
    {
        Header = header;
        Length = length;
        Data = data;
    }

    public SerializationData(byte header, byte[] data)
    {
        Header = header;
        Length = data.Length;
        Data = data;
    }
}
