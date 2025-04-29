using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Network.Interfaces.Packets;

namespace Prima.Core.Server.Interfaces.Services;

public interface INetworkService : IOrionService, IOrionStartService, IDisposable
{
    void RegisterPacketListener<TPacket>(INetworkPacketListener listener) where TPacket : IUoNetworkPacket, new();
}
