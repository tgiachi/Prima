using System.Net;
using Orion.Foundations.Spans;
using Prima.Network.Extensions;
using Prima.Network.Packets.Base;
using Prima.Network.Packets.Entries;

namespace Prima.Network.Packets;

/// <summary>
/// Game Server List packet (0xA8) - Sent by the server to provide a list of available game servers.
/// Contains information about each server including its name, load percentage, timezone, and IP address.
/// </summary>
public class GameServerList() : BaseUoNetworkPacket(0xA8, -1)
{
    /// <summary>
    /// System Information Flag - Different values control client behavior:
    /// 0xCC - Do not send video card info
    /// 0x64 - Send video card info
    /// 0x5D - Used by RunUO and SteamEngine
    /// Default is set to 0x5D to match common server implementations.
    /// </summary>
    public byte SystemInfoFlag { get; set; } = 0x5D;

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
    /// BYTE[2] Length (variable, calculated) - part of the packet header
    /// BYTE[1] SystemInfoFlag (0x5D, 0x64, or 0xCC)
    /// BYTE[2] Number of servers (big endian)
    /// For each server:
    ///   BYTE[2] Server index (0-based, big endian)
    ///   BYTE[32] Server name (fixed length, padded with nulls)
    ///   BYTE Percent full (load percentage)
    ///   BYTE Timezone
    ///   BYTE[4] Server IP address (reversed for network order)
    /// </summary>
    /// <param name="writer">The packet writer to write the data to.</param>
    public override Span<byte> Write()
    {
        using var writer = new SpanWriter(stackalloc byte[1], true);
        var servers = GetServers();

        writer.Write((ushort)(servers.Length + 6));

        // Write the system info flag
        writer.Write(SystemInfoFlag);

        // Write the number of servers
        writer.Write((ushort)Servers.Count);

        writer.Write(servers);

        return writer.ToSpan().Span;
    }

    private byte[] GetServers()
    {
        using var stream = new SpanWriter(stackalloc byte[Servers.Count * 40 + 6], true);
        for (var i = 0; i < Servers.Count; i++)
        {
            var server = Servers[i];

            // Write server index
            stream.Write((ushort)i);

            // Write server name (fixed 32 bytes)
            stream.WriteAscii(server.Name, 32);

            // Write load percentage
            stream.Write(server.LoadPercent);

            // Write timezone
            stream.Write(server.TimeZone);

            stream.Write(server.IP.ToRawAddress());
        }

        return stream.Span.ToArray();
    }

    /// <summary>
    /// Reads packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read the data from.</param>
    public override void Read(SpanReader reader)
    {
        reader.ReadByte();
        reader.ReadByte();


        SystemInfoFlag = reader.ReadByte();
        var serverCount = reader.ReadInt16();

        // Clear existing servers before reading new ones
        Servers.Clear();

        for (int i = 0; i < serverCount; i++)
        {
            var entry = new GameServerEntry
            {
                Index = reader.ReadUInt16(),
                Name = reader.ReadAscii(32),
                LoadPercent = reader.ReadByte(),
                TimeZone = reader.ReadByte()
            };

            // IP address bytes are reversed in the packet
            // For example, 0100A8C0 needs to be converted to 192.168.0.1
            byte[] ipBytes = new byte[4];
            reader.Read(ipBytes);
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
        return base.ToString() + $" {{ SystemInfoFlag: 0x{SystemInfoFlag:X2}, ServerCount: {Servers.Count} }}";
    }
}
