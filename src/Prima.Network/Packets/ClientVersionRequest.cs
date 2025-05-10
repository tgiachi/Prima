using System.Net;
using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;

namespace Prima.Network.Packets;

public class ClientVersionRequest : BaseUoNetworkPacket
{
    public int Seed { get; set; }

    public IPAddress ClientIP { get; set; }

    public int MajorVersion { get; set; }

    public int MinorVersion { get; set; }

    public int Revision { get; set; }

    public int Prototype { get; set; }


    public ClientVersionRequest() : base(0xEF, 21)
    {
    }

    public ClientVersionRequest(int majorVersion, int minorVersion, int revision, int prototype) : this()
    {
        MajorVersion = majorVersion;
        MinorVersion = minorVersion;
        Revision = revision;
        Prototype = prototype;
    }

    public override void Read(SpanReader reader)
    {
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
               $" {{ Seed: {Seed}, ClientIP: {ClientIP}, v{MajorVersion}.{MinorVersion}.{Revision}.{Prototype} }}";
    }
}
