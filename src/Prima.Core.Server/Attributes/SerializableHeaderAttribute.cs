namespace Prima.Core.Server.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SerializableHeaderAttribute(byte header) : Attribute
{
    public byte Header { get; } = header;


}
