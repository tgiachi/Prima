using Prima.Network.Interfaces.Packets;

namespace Prima.Core.Server.Interfaces.Listeners;

public interface INetworkPacketListener
{
    Task OnMessageReceived(string sessionId, IUoNetworkPacket packet);
}
