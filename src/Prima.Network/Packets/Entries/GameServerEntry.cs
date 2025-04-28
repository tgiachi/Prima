using System.Net;

namespace Prima.Network.Packets.Entries;

/// <summary>
/// Represents a game server entry in the server list.
/// Contains information about a single game server that clients can connect to.
/// </summary>
public class GameServerEntry
{
    /// <summary>
    /// Gets or sets the index of the server in the server list.
    /// This is a 0-based index used to identify the server when a client selects it.
    /// </summary>
    public ushort Index { get; set; }

    /// <summary>
    /// Gets or sets the name of the server.
    /// This is limited to 32 characters when transmitted over the network.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the load percentage of the server.
    /// Represents how full/busy the server is, from 0 to 100.
    /// </summary>
    public byte LoadPercent { get; set; }

    /// <summary>
    /// Gets or sets the timezone of the server.
    /// Used to group servers by geographic region.
    /// </summary>
    public byte TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the server.
    /// This is the address that clients will ping and connect to.
    /// </summary>
    public IPAddress IP { get; set; } = IPAddress.None;

    /// <summary>
    /// Returns a string representation of this server entry.
    /// </summary>
    /// <returns>A string containing the server index, name, load percentage, and IP address.</returns>
    public override string ToString()
    {
        return $"GameServerEntry {{ Index: {Index}, Name: {Name}, Load: {LoadPercent}%, IP: {IP} }}";
    }
}
