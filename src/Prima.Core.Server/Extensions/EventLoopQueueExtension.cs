using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;

namespace Prima.Core.Server.Extensions;

public static class EventLoopQueueExtension
{
    public static Guid EnqueueTaskLowPriority(this IEventLoopService eventLoopService, string name, Action action)
    {
        return eventLoopService.EnqueueAction(name, action, EventLoopPriority.Low);
    }

    public static Guid EnqueueTaskHighPriority(this IEventLoopService eventLoopService, string name, Action action)
    {
        return eventLoopService.EnqueueAction(name, action, EventLoopPriority.High);
    }
}
