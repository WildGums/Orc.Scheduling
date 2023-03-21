namespace Orc.Scheduling;

using System;

public static class RunningTaskExtensions
{
    public static bool IsExpired(this RunningTask runningTask, ITimeService timeService)
    {
        ArgumentNullException.ThrowIfNull(runningTask);
        ArgumentNullException.ThrowIfNull(timeService);

        var duration = timeService.CurrentDateTime - runningTask.Started;
        if (duration > runningTask.ScheduledTask.MaximumDuration)
        {
            return true;
        }

        return false;
    }
}
