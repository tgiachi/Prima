namespace Prima.Core.Server.Types;

/// <summary>
/// Defines priority levels for event loop actions.
/// </summary>
public enum EventLoopPriority
{
    /// <summary>
    /// Low priority actions are executed last.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority actions are executed after high priority actions.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority actions are executed first.
    /// </summary>
    High = 2
}
