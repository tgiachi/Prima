using Prima.Network.Packets.Base;

namespace Prima.UOData.Packets;

public class LoginComplete : BaseUoNetworkPacket
{
    public LoginComplete() : base(0x55, 1)
    {
    }
}
