namespace Prima.Core.Server.Data.Serialization;

public struct SerializationEntryData
{

    public byte Header;

    public long Length;

    public byte[] Data;

    public string FileName;

    public SerializationEntryData(byte header, long length, byte[] data, string fileName)
    {
        Header = header;
        Length = length;
        Data = data;
        FileName = fileName;
    }

    public SerializationEntryData(byte header, byte[] data, string fileName)
    {
        Header = header;
        Length = data.Length;
        Data = data;
        FileName = fileName;
    }
}
