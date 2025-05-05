using Orion.Core.Server.Interfaces.Sessions;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Packets;

namespace Prima.Core.Server.Data.Session;

public class NetworkSession : INetworkSession
{
    public delegate Task SendPacketDelegate(string id, IUoNetworkPacket packet);

    public delegate Task DisconnectDelegate(string id);

    public event SendPacketDelegate OnSendPacket;

    public event DisconnectDelegate OnDisconnect;

    public bool IsSeed { get; set; }

    public string Id { get; set; }

    public int Seed { get; set; }

    public bool FirstPacketReceived { get; set; }

    public int AuthId { get; set; }

    public string AccountId { get; set; }


    public ClientVersion ClientVersion { get; set; }


    public void Dispose()
    {
    }

    public void Initialize()
    {
        IsSeed = false;
        Id = string.Empty;
        Seed = 0;
        AuthId = 0;
    }

    public async Task Disconnect()
    {
        await OnDisconnect(Id);
    }

    public async Task SendPacketAsync(params IUoNetworkPacket[] packets)
    {
        foreach (var packet in packets)
        {
            await OnSendPacket(Id, packet);
        }
    }
}
