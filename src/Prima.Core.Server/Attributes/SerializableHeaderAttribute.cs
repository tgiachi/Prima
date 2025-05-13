namespace Prima.Core.Server.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SerializableHeaderAttribute(byte header, string fileName) : Attribute
{
    public byte Header { get; } = header;

    public string FileName { get; } = fileName;
}
