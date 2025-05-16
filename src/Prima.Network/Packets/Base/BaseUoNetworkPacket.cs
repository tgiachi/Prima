using Orion.Foundations.Spans;
using Prima.Network.Interfaces.Packets;

namespace Prima.Network.Packets.Base;

/// <summary>
/// Abstract base class for all Ultima Online network packets.
/// Provides common functionality and implements the IUoNetworkPacket interface.
/// </summary>
public abstract class BaseUoNetworkPacket : IUoNetworkPacket
{
    /// <summary>
    /// Gets the operation code that identifies the packet type.
    /// </summary>
    public byte OpCode { get; }


    /// <summary>
    ///  Gets the length of the packet data.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Initializes a new instance of the BaseUoNetworkPacket class with the specified operation code.
    /// </summary>
    /// <param name="opCode">The operation code that identifies this packet type.</param>
    protected BaseUoNetworkPacket(byte opCode, int length)
    {
        OpCode = opCode;
        Length = length;
    }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// Default implementation throws NotImplementedException and should be overridden.
    /// </summary>
    /// <param name="reader">The packet reader containing the packet data.</param>
    /// <exception cref="NotImplementedException">Thrown if this method is not overridden in derived classes.</exception>
    public virtual void Read(SpanReader reader)
    {

    }

    /// <summary>
    /// Writes the packet data to the provided packet writer.
    /// Default implementation throws NotImplementedException and should be overridden.
    /// </summary>
    /// <param name="writer">The packet writer to write packet data to.</param>
    /// <exception cref="NotImplementedException">Thrown if this method is not overridden in derived classes.</exception>
    public virtual Span<byte> Write()
    {
        throw new NotImplementedException("Write method not implemented for this packet type.");
    }

    /// <summary>
    /// Returns a string representation of this packet.
    /// </summary>
    /// <returns>A string containing the packet type name and OpCode.</returns>
    public override string ToString()
    {
        return GetType().Name + " { OpCode: 0x" + OpCode.ToString("X2") + " }";
    }
}
