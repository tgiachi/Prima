using System.Net;
using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

/// <summary>
/// Represents a connect to game server packet sent from the login server to the client
/// after a shard selection has been accepted.
/// </summary>
/// <remarks>
/// OpCode: 0x8C
/// This packet provides the client with connection information for the selected game server.
/// </remarks>
public class ConnectToGameServer() : BaseUoNetworkPacket(0x8c, 11)
{
    /// <summary>
    /// Gets or sets the IP address of the game server to connect to.
    /// </summary>
    public IPAddress GameServerIP { get; set; } = IPAddress.None;

    /// <summary>
    /// Gets or sets the port of the game server to connect to.
    /// </summary>
    public ushort GameServerPort { get; set; }

    /// <summary>
    /// Gets or sets the session key to authenticate with the game server.
    /// This ensures the connection is coming from a properly authenticated client.
    /// </summary>
    public uint SessionKey { get; set; }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public override void Read(PacketReader reader)
    {
        byte[] ipBytes = reader.ReadBytes(4);
        GameServerIP = new IPAddress(ipBytes);
        GameServerPort = reader.ReadUInt16();
        SessionKey = reader.ReadUInt32();
    }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write data to.</param>
    public override void Write(PacketWriter writer)
    {
        writer.WriteIpAddress(GameServerIP);
        writer.Write(GameServerPort);
        writer.Write(SessionKey);
    }
}
