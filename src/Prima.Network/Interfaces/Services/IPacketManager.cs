using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Network.Interfaces.Packets;

namespace Prima.Network.Interfaces.Services;

/// <summary>
/// Defines the interface for a packet manager service.
/// The packet manager handles packet registration, serialization, and deserialization.
/// </summary>
public interface IPacketManager : IOrionService
{
    /// <summary>
    /// Registers a packet type with the packet manager.
    /// </summary>
    /// <typeparam name="T">The type of packet to register, must implement IUoNetworkPacket and have a parameterless constructor.</typeparam>
    void RegisterPacket<T>() where T : IUoNetworkPacket, new();

    /// <summary>
    /// Serializes a packet to a byte array.
    /// </summary>
    /// <typeparam name="T">The type of packet to serialize.</typeparam>
    /// <param name="packet">The packet to serialize.</param>
    /// <returns>A byte array containing the serialized packet data.</returns>
    byte[] WritePacket<T>(T packet) where T : IUoNetworkPacket;

    /// <summary>
    /// Deserializes a byte array into a packet.
    /// </summary>
    /// <param name="data">The byte array containing the packet data.</param>
    /// <returns>The deserialized packet, or null if the packet type is not registered.</returns>
    IUoNetworkPacket? ReadPacket(byte[] data);
}
