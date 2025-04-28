using System.Net;
using Prima.Network.Packets.Base;
using Prima.Network.Packets.Entries;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

public class GameServerList() : BaseUoNetworkPacket(0xA8)
{
    public byte Command { get; set; }
    public byte SystemInfoFlag { get; set; } = 0x05;

    public List<GameServerEntry> Servers { get; } = new();


    public void AddServer(GameServerEntry server)
    {
        if (Servers.Count < 255)
        {
            Servers.Add(server);
        }
    }

    public override void Write(PacketWriter writer)
    {
        writer.Write(SystemInfoFlag);
        writer.WriteUInt16BE((ushort)Servers.Count);

        foreach (var server in Servers)
        {
            writer.WriteUInt16BE(server.Index);
            writer.WriteFixedString(server.Name, 32);
            writer.Write(server.LoadPercent);
            writer.Write(server.TimeZone);


            var ipBytes = server.IP.GetAddressBytes();
            Array.Reverse(ipBytes);
            writer.Write(ipBytes);
        }
    }

    public void Read(PacketReader reader)
    {
        SystemInfoFlag = reader.ReadByte();
        ushort serverCount = reader.ReadUInt16BE();

        for (int i = 0; i < serverCount; i++)
        {
            var entry = new GameServerEntry
            {
                Index = reader.ReadUInt16BE(),
                Name = reader.ReadFixedString(32),
                LoadPercent = reader.ReadByte(),
                TimeZone = reader.ReadByte()
            };

            byte[] ipBytes = reader.ReadBytes(4);
            Array.Reverse(ipBytes);
            entry.IP = new IPAddress(ipBytes);

            Servers.Add(entry);
        }
    }
}
