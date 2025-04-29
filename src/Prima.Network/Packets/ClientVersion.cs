using System.Net;
using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class ClientVersion : BaseUoNetworkPacket
{
    public byte Command { get; set; }

    public ushort Seed { get; set; }

    public IPAddress ClientIP { get; set; }

    public ushort MajorVersion { get; set; }

    public ushort MinorVersion { get; set; }

    public ushort Revision { get; set; }

    public ushort Prototype { get; set; }


    public ClientVersion() : base(0xEF, 21)
    {
    }

    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();

        // IP address bytes are reversed in the packet
        var ipBytes = reader.ReadBytes(4);
        Array.Reverse(ipBytes);
        ClientIP = new IPAddress(ipBytes);

        //bytes to ushort

        Seed = BitConverter.ToUInt16(ipBytes, 0);

        MajorVersion = reader.ReadUInt16BE();
        MinorVersion = reader.ReadUInt16BE();
        Revision = reader.ReadUInt16BE();
        Prototype = reader.ReadUInt16BE();
    }


    public override string ToString()
    {
        return base.ToString() +
               $" {{ Command: {Command:X2}, Seed: {Seed}, ClientIP: {ClientIP}, v{MajorVersion}.{MinorVersion}.{Revision}.{Prototype} }}";
    }
}
