using System.Net;
using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class ConnectToGameServer() : BaseUoNetworkPacket(0x8C)
{
    public byte Command { get; set; }
    public IPAddress GameServerIP { get; set; }
    public ushort GameServerPort { get; set; }
    public uint SessionKey { get; set; }

    public override void Read(PacketReader reader)
    {
        byte[] ipBytes = reader.ReadBytes(4);
        GameServerIP = new IPAddress(ipBytes);
        GameServerPort = reader.ReadUInt16BE();
        SessionKey = reader.ReadUInt32BE();
    }

    public override void Write(PacketWriter writer)
    {
        writer.Write(GameServerIP.GetAddressBytes());
        writer.WriteUInt16BE(GameServerPort);
        writer.WriteUInt32BE(SessionKey);
    }
}
