using System.Net;
using Prima.Network.Packets.Base;
using Prima.Network.Packets.Entries;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

/// <summary>
/// Game Server List packet (0xA8) - Sent by the server to provide a list of available game servers.
/// Contains information about each server including its name, load percentage, timezone, and IP address.
/// </summary>
public class GameServerList() : BaseUoNetworkPacket(0xA8, -1)
{
    /// <summary>
    /// The command identifier for this packet.
    /// </summary>
    public byte Command { get; set; } = 0xA8;

    /// <summary>
    /// System Information Flag - typically set to 0x05.
    /// </summary>
    public byte SystemInfoFlag { get; set; } = 0x05;

    /// <summary>
    /// Collection of game servers to be included in the list.
    /// Maximum of 255 servers can be included.
    /// </summary>
    public List<GameServerEntry> Servers { get; } = new();

    /// <summary>
    /// Adds a server to the list of available game servers.
    /// Only adds the server if the list contains fewer than 255 servers.
    /// </summary>
    /// <param name="server">The server entry to add to the list.</param>
    public void AddServer(GameServerEntry server)
    {
        if (Servers.Count < 255)
        {
            Servers.Add(server);
        }
    }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    ///
    /// Packet Structure:
    /// BYTE[1] Command (0xA8)
    /// BYTE[1] SystemInfoFlag (0x05)
    /// BYTE[2] Number of servers (big endian)
    /// For each server:
    ///   BYTE[2] Server index (0-based, big endian)
    ///   BYTE[32] Server name (fixed length, padded with nulls)
    ///   BYTE Percent full (load percentage)
    ///   BYTE Timezone
    ///   BYTE[4] Server IP address (reversed for network order)
    /// </summary>
    /// <param name="writer">The packet writer to write the data to.</param>
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

            // IP address bytes need to be reversed for network byte order
            var ipBytes = server.IP.GetAddressBytes();
            Array.Reverse(ipBytes);
            writer.Write(ipBytes);
        }
    }

    /// <summary>
    /// Reads packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read the data from.</param>
    public override void Read(PacketReader reader)
    {
        SystemInfoFlag = reader.ReadByte();
        ushort serverCount = reader.ReadUInt16BE();

        // Clear existing servers before reading new ones
        Servers.Clear();

        for (int i = 0; i < serverCount; i++)
        {
            var entry = new GameServerEntry
            {
                Index = reader.ReadUInt16BE(),
                Name = reader.ReadFixedString(32),
                LoadPercent = reader.ReadByte(),
                TimeZone = reader.ReadByte()
            };

            // IP address bytes are reversed in the packet
            byte[] ipBytes = reader.ReadBytes(4);
            Array.Reverse(ipBytes);
            entry.IP = new IPAddress(ipBytes);

            Servers.Add(entry);
        }
    }

    /// <summary>
    /// Returns a string representation of this packet.
    /// </summary>
    /// <returns>A string containing the packet type, OpCode, and number of servers.</returns>
    public override string ToString()
    {
        return base.ToString() + $" {{ ServerCount: {Servers.Count} }}";
    }
}
