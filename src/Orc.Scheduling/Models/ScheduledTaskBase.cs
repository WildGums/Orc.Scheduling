// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledTaskBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;

    public abstract class ScheduledTaskBase : IScheduledTask
    {
        protected ScheduledTaskBase()
        {
            MaximumDuration = TimeSpan.MaxValue;
            ScheduleRecurringTaskAfterTaskExecutionHasCompleted = false;
        }

        public string Name { get; set; }

        public DateTime Start { get; set; }

        public TimeSpan? Recurring { get; set; }

        public TimeSpan MaximumDuration { get; set; }

        public bool ScheduleRecurringTaskAfterTaskExecutionHasCompleted { get; set; }

        public abstract Task InvokeAsync();

        public override string ToString()
        {
            var value = string.Format("{0} | Start at {1} | {2} | {3}", Name, Start, 
                MaximumDuration < TimeSpan.MaxValue ? string.Format("Maximum duration is {0}", MaximumDuration) : "No maximum duration",
                Recurring.HasValue ? string.Format("Recurs every {0}", Recurring) : "Not recurring");
            return value;
        }

        public abstract IScheduledTask Clone();
    }
}