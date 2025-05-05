using Prima.Network.Packets.Base;
using Prima.Network.Serializers;

namespace Prima.Network.Packets;

/// <summary>
/// Represents a login request packet sent from the client to the login server.
/// This packet contains the credentials for authentication.
/// </summary>
/// <remarks>
/// OpCode: 0x80
/// </remarks>
public class LoginRequest() : BaseUoNetworkPacket(0x80, 62)
{
    /// <summary>
    /// Gets or sets the account username.
    /// Length is fixed at 30 characters, padded with null terminators.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the account password.
    /// Length is fixed at 30 characters, padded with null terminators.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the next login key for session management.
    /// </summary>
    public byte NextLoginKey { get; set; }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write data to.</param>
    public override void Write(PacketWriter writer)
    {
        writer.WriteAsciiFixed(Username, 30);
        writer.WriteAsciiFixed(Password, 30);
        writer.Write(NextLoginKey);
    }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public override void Read(PacketReader reader)
    {
        Username = reader.ReadFixedString(30);
        Password = reader.ReadFixedString(30);
        NextLoginKey = reader.ReadByte();
    }

    /// <summary>
    /// Returns a string representation of this packet for debugging purposes.
    /// </summary>
    /// <returns>A string representation of the packet and its properties.</returns>
    public override string ToString()
    {
        return
            $"{base.ToString()} {{  Username: {Username}, Password: {Password}, NextLoginKey: {NextLoginKey} }}";
    }
}
