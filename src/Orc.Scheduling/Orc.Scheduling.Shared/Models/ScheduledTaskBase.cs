// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledTaskBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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
        }

        public string Name { get; set; }

        public DateTime Start { get; set; }

        public TimeSpan? Recurring { get; set; }

        public TimeSpan MaximumDuration { get; set; }

        public abstract Task InvokeAsync();

        public override string ToString()
        {
            var value = string.Format("Name {0} | Start at {1} | Maximum duration is {2} | Recur every {3}", Name, Start, MaximumDuration, Recurring);
            return value;
        }

        public abstract object Clone();
    }
}