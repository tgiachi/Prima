using Orion.Core.Server.Interfaces.Sessions;
using Prima.Core.Server.Data.Uo;
using Prima.Network.Interfaces.Packets;

namespace Prima.Core.Server.Data.Session;

public class NetworkSession : INetworkSession
{
    public delegate Task SendPacketDelegate(string id, IUoNetworkPacket packet);

    public delegate Task DisconnectDelegate(string id);


    private readonly Dictionary<string, object> _properties = new();


    public void SetProperty<T>(T value, string name = "default")
    {
        if (_properties.ContainsKey(name))
        {
            _properties[name] = value;
        }
        else
        {
            _properties.Add(name, value);
        }
    }

    public T GetProperty<T>(string name = "default")
    {
        if (_properties.TryGetValue(name, out var value))
        {
            return (T)value;
        }

        return default;
    }

    public DateTime LastPing { get; set; }

    public event SendPacketDelegate OnSendPacket;
    public event DisconnectDelegate OnDisconnect;

    public bool UseNetworkCompression { get; set; }

    public bool IsSeed { get; set; }

    public string Id { get; set; }

    public int Seed { get; set; }

    public bool FirstPacketReceived { get; set; }

    public uint AuthId { get; set; }

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
        AccountId = string.Empty;
        ClientVersion = null;

        FirstPacketReceived = false;
        LastPing = DateTime.MinValue;
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
