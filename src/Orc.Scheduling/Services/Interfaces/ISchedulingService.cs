// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Collections.Generic;

    public interface ISchedulingService
    {
        #region Properties
        bool IsEnabled { get; }

        [ObsoleteEx(ReplacementTypeOrMember = "GetScheduledTasks", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        List<IScheduledTask> ScheduledTasks { get; }

        [ObsoleteEx(ReplacementTypeOrMember = "GetRunningTasks", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        List<RunningTask> RunningTasks { get; }
        #endregion

        event EventHandler<TaskEventArgs> TaskStarted;
        event EventHandler<TaskEventArgs> TaskCanceled;
        event EventHandler<TaskEventArgs> TaskCompleted;

        List<IScheduledTask> GetScheduledTasks();
        List<RunningTask> GetRunningTasks();

        void Start();
        void Stop();

        void AddScheduledTask(IScheduledTask scheduledTask);
        void RemoveScheduledTask(IScheduledTask scheduledTask);
    }
}
