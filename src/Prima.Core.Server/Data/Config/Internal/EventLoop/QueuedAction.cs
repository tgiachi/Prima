using Prima.Core.Server.Types;

namespace Prima.Core.Server.Data.Config.Internal.EventLoop;

/// <summary>
/// Represents an action queued in the event loop.
/// </summary>
public struct QueuedAction
{
    /// <summary>
    /// Gets the unique identifier for this action.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the name of the action for easier identification.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the action to be executed.
    /// </summary>
    public Action Action { get; }

    /// <summary>
    /// Gets the priority of the action.
    /// </summary>
    public EventLoopPriority Priority { get; }

    /// <summary>
    /// Gets the timestamp when the action was enqueued.
    /// </summary>
    public DateTime EnqueuedAt { get; }

    /// <summary>
    /// Gets the timestamp when execution started.
    /// </summary>
    public long ExecutionStartTimestamp { get; set; }

    /// <summary>
    /// Gets the timestamp when execution ended.
    /// </summary>
    public long ExecutionEndTimestamp { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueuedAction"/> struct.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to be executed.</param>
    /// <param name="priority">The priority of the action.</param>
    public QueuedAction(string name, Action action, EventLoopPriority priority)
    {
        Id = Guid.NewGuid();
        Name = name;
        Action = action;
        Priority = priority;
        EnqueuedAt = DateTime.UtcNow;
        ExecutionStartTimestamp = 0;
        ExecutionEndTimestamp = 0;
    }
}
