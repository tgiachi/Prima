using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;


namespace Prima.Network.Packets;

public class GameServerLogin : BaseUoNetworkPacket
{
    public int SessionKey { get; set; }

    public string AccountId { get; set; }

    public string Password { get; set; }

    public GameServerLogin() : base(0x91, 65)
    {
    }

    public override void Read(SpanReader reader)
    {
        SessionKey = reader.ReadInt32();
        AccountId = reader.ReadAscii(30);
        Password = reader.ReadAscii(30);
    }
}
