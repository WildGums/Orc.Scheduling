// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISchedulingService
    {
        #region Properties
        bool IsEnabled { get; }

        List<IScheduledTask> ScheduledTasks { get; }

        List<RunningTask> RunningTasks { get; }
        #endregion

        event EventHandler<TaskEventArgs> TaskStarted;
        event EventHandler<TaskEventArgs> TaskCanceled;
        event EventHandler<TaskEventArgs> TaskCompleted;

        void Start();
        void Stop();

        void AddScheduledTask(IScheduledTask scheduledTask);
        void RemoveScheduledTask(IScheduledTask scheduledTask);
    }
}