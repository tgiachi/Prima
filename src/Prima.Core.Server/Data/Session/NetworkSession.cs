using Orion.Core.Server.Interfaces.Sessions;

namespace Prima.Core.Server.Data.Session;

public class NetworkSession : INetworkSession
{
    public bool IsAuthenticated { get; set; }

    public string Id { get; set; }

    public void Dispose()
    {
    }

    public void Initialize()
    {
        IsAuthenticated = false;
    }
}
