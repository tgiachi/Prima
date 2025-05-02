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
public class LoginDenied() : BaseUoNetworkPacket(0x82, 1)
{
    /// <summary>
    /// Gets or sets the reason why the login was denied.
    /// </summary>
    public LoginDeniedReasonType Reason { get; set; }


    public LoginDenied(LoginDeniedReasonType reason) : this()
    {
        Reason = reason;
    }



    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// </summary>
    /// <param name="writer">The packet writer to write data to.</param>
    public override void Write(PacketWriter writer)
    {
        writer.WriteEnum(Reason);
    }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public override void Read(PacketReader reader)
    {
        Reason = reader.ReadEnum<LoginDeniedReasonType>();
    }
}
