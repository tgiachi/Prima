using Prima.Network.Packets.Base;
using Prima.Network.Serializers;
using Prima.Network.Types;

namespace Prima.Network.Packets;

public class LoginDenied : BaseUoNetworkPacket
{
    public byte Command { get; set; }

    public LoginDeniedReasonType Reason { get; set; }

    public LoginDenied() : base(0x82)
    {
    }

    public override void Write(PacketWriter writer)
    {
        writer.WriteByte(Command);
        writer.WriteEnum(Reason);
    }

    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();
        Reason = reader.ReadEnum<LoginDeniedReasonType>();
    }
}
