using Orion.Core.Server.Interfaces.Sessions;
using Prima.Network.Interfaces.Packets;

namespace Prima.Core.Server.Data.Session;

public class NetworkSession : INetworkSession
{
    public delegate Task SendPacketDelegate(string id, IUoNetworkPacket packet);

    public event SendPacketDelegate OnSendPacket;

    public bool IsSeed { get; set; }

    public string Id { get; set; }

    public int Seed { get; set; }

    public string ClientVersion { get; set; }


    public void Dispose()
    {
    }

    public void Initialize()
    {
        IsSeed = false;
        Id = string.Empty;
        Seed = 0;
    }

    public async Task SendPacketAsync(params IUoNetworkPacket[] packets)
    {
        foreach (var packet in packets)
        {
            await OnSendPacket(Id, packet);
        }
    }
}
