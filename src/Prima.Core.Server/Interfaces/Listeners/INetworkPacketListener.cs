using Prima.Network.Interfaces.Packets;

namespace Prima.Core.Server.Interfaces.Listeners;

public interface INetworkPacketListener
{
    Task OnPacketReceived(string sessionId, IUoNetworkPacket packet);
}


public interface INetworkPacketListener<in TPacket> where TPacket : IUoNetworkPacket
{
    Task OnPacketReceived(string sessionId, TPacket packet);
}
