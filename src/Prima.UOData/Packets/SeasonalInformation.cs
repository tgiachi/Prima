using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;
using Prima.UOData.Types;

namespace Prima.UOData.Packets;

public class SeasonalInformation : BaseUoNetworkPacket
{
    public Season Season { get; set; }

    public bool PlaySound { get; set; }

    public SeasonalInformation(Season season, bool playSound) : this()
    {
    }

    public SeasonalInformation() : base(0xBC, 3)
    {
    }

    public override Span<byte> Write()
    {
        using var packetWriter = new SpanWriter(stackalloc byte[3]);

        packetWriter.Write((byte)Season);
        packetWriter.Write((byte)(PlaySound ? 1 : 0));

        return packetWriter.Span.ToArray();
    }
}
