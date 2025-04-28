using Prima.Network.Interfaces.Packets;

namespace Prima.Network.Interfaces.Services;

public interface IPacketManager
{
    void RegisterPacket<T>() where T : IUoNetworkPacket, new();

    byte[] WritePacket<T>(T packet) where T : IUoNetworkPacket;

    IUoNetworkPacket ReadPacket(byte[] data);
}
