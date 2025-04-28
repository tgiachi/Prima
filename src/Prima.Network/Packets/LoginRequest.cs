using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class LoginRequest : BaseUoNetworkPacket
{
    public byte Command { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public byte NextLoginKey { get; set; }

    public LoginRequest() : base(0x80)
    {
    }


    public override void Write(PacketWriter writer)
    {
        writer.WriteByte(Command);
        writer.WriteFixedString(Username, 30);
        writer.WriteFixedString(Password, 30);
        writer.WriteByte(NextLoginKey);
    }

    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();
        Username = reader.ReadFixedString(30);
        Password = reader.ReadFixedString(30);
        NextLoginKey = reader.ReadByte();
    }

    public override string ToString()
    {
        return
            $"{base.ToString()} {{ Command: {Command}, Username: {Username}, Password: {Password}, NextLoginKey: {NextLoginKey} }}";
    }
}
