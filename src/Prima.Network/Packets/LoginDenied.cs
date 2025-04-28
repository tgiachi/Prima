using Prima.Network.Packets.Base;
using Prima.Network.Serializers;
using Prima.Network.Types;

namespace Prima.Network.Packets;

/// <summary>
/// Represents a login denied packet sent from the server to the client
/// when authentication fails.
/// </summary>
/// <remarks>
/// OpCode: 0x82
/// </remarks>
public class LoginDenied() : BaseUoNetworkPacket(0x82)
{
    /// <summary>
    /// Gets or sets the command byte for this packet.
    /// </summary>
    public byte Command { get; set; }

    /// <summary>
    /// Gets or sets the reason why the login was denied.
    /// </summary>
    public LoginDeniedReasonType Reason { get; set; }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write data to.</param>
    public override void Write(PacketWriter writer)
    {
        writer.WriteByte(Command);
        writer.WriteEnum(Reason);
    }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public override void Read(PacketReader reader)
    {
        Command = reader.ReadByte();
        Reason = reader.ReadEnum<LoginDeniedReasonType>();
    }
}
