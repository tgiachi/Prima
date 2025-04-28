using System.Net;

namespace Prima.Network.Packets.Entries;

public class GameServerEntry
{
    public ushort Index { get; set; }
    public string Name { get; set; }
    public byte LoadPercent { get; set; }
    public byte TimeZone { get; set; }
    public IPAddress IP { get; set; }
}
