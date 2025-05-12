using Prima.Network.Packets.Base;

namespace Prima.Network.Packets;

public class ClientVersionReq : BaseUoNetworkPacket
{
    public ClientVersionReq() : base(0xBD, 3)
    {
    }

    public override Span<byte> Write()
    {
        return new byte[] { 0x00, 0x03 };
    }
}
