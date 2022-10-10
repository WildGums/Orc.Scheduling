namespace Orc.Scheduling
{
    using System;
    using System.Collections.Generic;

    public interface ISchedulingService
    {
        bool IsEnabled { get; }

        event EventHandler<TaskEventArgs>? TaskStarted;
        event EventHandler<TaskEventArgs>? TaskCanceled;
        event EventHandler<TaskEventArgs>? TaskCompleted;

        List<IScheduledTask> GetScheduledTasks();
        List<RunningTask> GetRunningTasks();

        void Start();
        void Stop();

        void AddScheduledTask(IScheduledTask scheduledTask);
        void RemoveScheduledTask(IScheduledTask scheduledTask);
    }
}
