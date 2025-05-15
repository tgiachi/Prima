using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Attributes.Scripts;

namespace Prima.Server.Modules.Scripts;

[ScriptModule("scheduler")]
public class SchedulerModule
{
    private readonly ISchedulerSystemService _schedulerSystemService;

    public SchedulerModule(ISchedulerSystemService schedulerSystemService)
    {
        _schedulerSystemService = schedulerSystemService;
    }

    [ScriptFunction("Schedule a task to be run every x seconds")]
    public void ScheduleTask(string name, int seconds, Action callback)
    {
        _schedulerSystemService.RegisterJob(
            name,
            () =>
            {
                callback();
                return Task.CompletedTask;
            },
            TimeSpan.FromSeconds(seconds)
        );
    }
}
