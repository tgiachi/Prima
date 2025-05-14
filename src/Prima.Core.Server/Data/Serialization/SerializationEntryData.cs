namespace Prima.Core.Server.Data.Serialization;

public struct SerializationEntryData
{
    public readonly byte Header;

    public readonly long Length;

    public readonly byte[] Data;


    public SerializationEntryData(byte header, long length, byte[] data)
    {
        Header = header;
        Length = length;
        Data = data;
    }

    public SerializationEntryData(byte header, byte[] data)
    {
        Header = header;
        Length = data.Length;
        Data = data;
    }
}
