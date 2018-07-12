// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTask.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;

    public interface IScheduledTask
    {
        string Name { get; set; }
        DateTime Start { get; set; }
        TimeSpan? Recurring { get; set; }
        bool ScheduleRecurringTaskAfterTaskExecutionHasCompleted { get; set; }

        TimeSpan MaximumDuration { get; set; }

        Task InvokeAsync();

        IScheduledTask Clone();
    }
}