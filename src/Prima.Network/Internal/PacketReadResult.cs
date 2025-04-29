using Prima.Network.Interfaces.Packets;

namespace Prima.Network.Internal;

/// <summary>
/// Represents the result of a packet read operation.
/// </summary>
public readonly struct PacketReadResult
{
    /// <summary>
    /// Gets the parsed packet, if successful.
    /// </summary>
    public IUoNetworkPacket Packet { get; }

    /// <summary>
    /// Gets the number of bytes consumed from the buffer.
    /// </summary>
    public int ConsumedBytes { get; }

    /// <summary>
    /// Gets whether the read operation was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Initializes a new instance of the PacketReadResult struct.
    /// </summary>
    public PacketReadResult(IUoNetworkPacket packet, int consumedBytes, bool success)
    {
        Packet = packet;
        ConsumedBytes = consumedBytes;
        Success = success;
    }

    /// <summary>
    /// Creates a failed read result.
    /// </summary>
    public static PacketReadResult Failed() => new(null, 0, false);
}
