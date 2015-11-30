// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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
        List<RunningTask> RunningTasks { get; }
        #endregion

        event EventHandler<TaskEventArgs> TaskStarted;
        event EventHandler<TaskEventArgs> TaskCanceled;
        event EventHandler<TaskEventArgs> TaskCompleted;
        void Start();
        void Stop();
    }
}