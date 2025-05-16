using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;

namespace Prima.UOData.Packets;

public class CharacterWarMode : BaseUoNetworkPacket
{
    public bool IsWarMode { get; set; }

    public CharacterWarMode() : base(0x72, 5)
    {
    }

    public CharacterWarMode(bool isWarMode = false) : this()
    {
        IsWarMode = isWarMode;
    }


    public override Span<byte> Write()
    {
        using var packetWriter = new SpanWriter(stackalloc byte[Length - 4], true);

        packetWriter.Write(IsWarMode);
        packetWriter.Write((byte)0);
        packetWriter.Write((byte)0x32);
        packetWriter.Write((byte)0);

        return packetWriter.Span.ToArray();
    }
}
