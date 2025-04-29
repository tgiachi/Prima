using System.Net;
using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class ClientVersion : BaseUoNetworkPacket
{
    public byte Command { get; set; }

    public int Seed { get; set; }

    public IPAddress ClientIP { get; set; }

    public int MajorVersion { get; set; }

    public int MinorVersion { get; set; }

    public int Revision { get; set; }

    public int Prototype { get; set; }


    public ClientVersion() : base(0xEF, 21)
    {
    }

    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();

        Seed = reader.ReadInt32();

        ClientIP = new IPAddress(Seed);

        MajorVersion = reader.ReadInt32();
        MinorVersion = reader.ReadInt32();
        Revision = reader.ReadInt32();
        Prototype = reader.ReadInt32();
    }


    public override string ToString()
    {
        return base.ToString() +
               $" {{ Command: {Command:X2}, Seed: {Seed}, ClientIP: {ClientIP}, v{MajorVersion}.{MinorVersion}.{Revision}.{Prototype} }}";
    }
}
