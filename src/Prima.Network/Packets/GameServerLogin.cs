using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class GameServerLogin : BaseUoNetworkPacket
{
    public int SessionKey { get; set; }

    public string AccountId { get; set; }

    public string Password { get; set; }

    public GameServerLogin() : base(0x91, 65)
    {
    }

    public override void Read(PacketReader reader)
    {
        SessionKey = reader.ReadInt32();
        AccountId = reader.ReadFixedString(30);
        Password = reader.ReadFixedString(30);
    }
}
