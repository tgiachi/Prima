using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Interfaces.Packets;

namespace Prima.Core.Server.Handlers.Base;

public abstract class BasePacketListenerHandler : INetworkPacketListener, IOrionService
{
    private readonly Dictionary<Type, object> _packetHandlers = new();

    private readonly INetworkService _networkService;
    private readonly IServiceProvider _serviceProvider;


    protected ILogger Logger { get; }

    protected abstract void RegisterHandlers();


    protected BasePacketListenerHandler(
        ILogger<BasePacketListenerHandler> logger, INetworkService networkService, IServiceProvider serviceProvider
    )
    {
        Logger = logger;
        _networkService = networkService;
        _serviceProvider = serviceProvider;
        RegisterHandlers();
    }


    public Task OnPacketReceived(string sessionId, IUoNetworkPacket packet)
    {
        var packetType = packet.GetType();
        if (_packetHandlers.TryGetValue(packetType, out var handlerObj))
        {
            var handlerInterfaceType = typeof(INetworkPacketListener<>).MakeGenericType(packetType);
            var methodInfo = handlerInterfaceType.GetMethod(nameof(OnPacketReceived), [typeof(string), packetType]);

            if (methodInfo != null)
            {
                return (Task)methodInfo.Invoke(handlerObj, [sessionId, packet]);
            }

            Logger.LogWarning(
                "Handler for packet type {PacketType} does not implement OnPacketReceived method., have you forgot to register with RegisterHandler ?",
                packetType
            );
        }


        return Task.CompletedTask;
    }

    protected void RegisterHandler<TPacket>(INetworkPacketListener<TPacket> handler) where TPacket : IUoNetworkPacket, new()
    {
        _packetHandlers[typeof(TPacket)] = handler;
        Logger.LogDebug("{PacketType} registered in {ClassType}", typeof(TPacket), GetType());
        _networkService.RegisterPacketListener<TPacket>(this);
    }

    protected NetworkSession? GetSession(string sessionId)
    {
        return _serviceProvider.GetRequiredService<INetworkSessionService<NetworkSession>>().GetSession(sessionId);
    }
}
