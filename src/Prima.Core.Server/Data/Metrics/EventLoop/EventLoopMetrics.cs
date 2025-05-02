namespace Prima.Core.Server.Data.Metrics.EventLoop;

/// <summary>
/// Contains metrics about the event loop performance.
/// </summary>
public class EventLoopMetrics
{

    /// <summary>
    /// Gets or sets the total number of ticks processed.
    /// </summary>
    public long TotalTicksProcessed { get; set; }

    /// <summary>
    /// Gets or sets accumulated tick processing time for calculation of averages.
    /// </summary>
    public double AccumulatedTickTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the number of actions processed in the current tick.
    /// </summary>
    public int ActionsProcessedInTick { get; set; }

    /// <summary>
    /// Gets or sets the total number of actions processed.
    /// </summary>
    public long TotalActionsProcessed { get; set; }

    /// <summary>
    /// Gets or sets the average time in milliseconds to process an action.
    /// </summary>
    public double AverageProcessingTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the maximum time in milliseconds spent processing a single action.
    /// </summary>
    public double MaxProcessingTimeMs { get; set; }

    /// <summary>
    ///  Get avarage time between ticks in milliseconds.
    /// </summary>
    public double AverageTimeBetweenTicksMs { get; set; }

    /// <summary>
    /// Gets or sets the current number of queued actions.
    /// </summary>
    public int QueuedActionsCount { get; set; }

    /// <summary>
    /// Gets or sets the time in milliseconds spent on the last tick.
    /// </summary>
    public double LastTickTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the current number of high priority actions in the queue.
    /// </summary>
    public int HighPriorityCount { get; set; }

    /// <summary>
    /// Gets or sets the current number of normal priority actions in the queue.
    /// </summary>
    public int NormalPriorityCount { get; set; }

    /// <summary>
    /// Gets or sets the current number of low priority actions in the queue.
    /// </summary>
    public int LowPriorityCount { get; set; }
}
