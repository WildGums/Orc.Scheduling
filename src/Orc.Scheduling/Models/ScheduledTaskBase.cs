﻿namespace Orc.Scheduling;

using System;
using System.Threading.Tasks;

public abstract class ScheduledTaskBase : IScheduledTask
{
    private string? _id;

    protected ScheduledTaskBase()
    {
        Name = string.Empty;
        MaximumDuration = TimeSpan.MaxValue;
        ScheduleRecurringTaskAfterTaskExecutionHasCompleted = false;
    }

    public virtual string Id
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                return Name;
            }

            return _id;
        }
        protected set => _id = value;
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
