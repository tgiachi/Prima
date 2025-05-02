namespace Prima.Core.Server.Data.Config.Internal.EventLoop;

/// <summary>
/// Configuration options for the Event Loop service.
/// </summary>
public class EventLoopConfig
{
    /// <summary>
    /// Gets or sets the interval in milliseconds between each tick of the event loop.
    /// Default is 50ms (20 ticks per second).
    /// </summary>
    public int TickIntervalMs { get; set; } = 50;

    /// <summary>
    /// Gets or sets the maximum number of actions to process in a single tick.
    /// Default is 100 to prevent the event loop from blocking for too long.
    /// </summary>
    public int MaxActionsPerTick { get; set; } = 100;

    /// <summary>
    /// Gets or sets the threshold in milliseconds for considering an action as slow.
    /// Actions that take longer will be logged as warnings.
    /// </summary>
    public int SlowActionThresholdMs { get; set; } = 50;

    /// <summary>
    /// Gets or sets whether to enable detailed metrics collection.
    /// </summary>
    public bool EnableDetailedMetrics { get; set; } = true;
}
