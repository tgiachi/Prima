using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;

namespace Prima.UOData.Packets;

public class GlobalLightLevel : BaseUoNetworkPacket
{
    public int LightLevel { get; set; }

    public GlobalLightLevel() : base(0x4F, 2)
    {
    }


    public GlobalLightLevel(int lightLevel) : this()
    {
        LightLevel = lightLevel;
    }

    public override Span<byte> Write()
    {
        using var packetWriter = new SpanWriter(stackalloc byte[2], true);
        packetWriter.Write((sbyte)LightLevel);
        return packetWriter.Span.ToArray();
    }
}
