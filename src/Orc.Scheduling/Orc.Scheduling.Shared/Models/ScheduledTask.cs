﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledTask.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using Catel;

    public class ScheduledTask
    {
        public ScheduledTask()
        {
            MaximumDuration = TimeSpan.MaxValue;
        }

        public ScheduledTask(ScheduledTask scheduledTask)
        {
            Argument.IsNotNull(() => scheduledTask);

            Name = scheduledTask.Name;
            Action = scheduledTask.Action;
            Start = scheduledTask.Start;
            Recurring = scheduledTask.Recurring;
            MaximumDuration = scheduledTask.MaximumDuration;
        }

        public string Name { get; set; }

        public Func<Task> Action { get; set; } 

        public DateTime Start { get; set; }

        public TimeSpan? Recurring { get; set; }

        public TimeSpan MaximumDuration { get; set; }

        public override string ToString()
        {
            var value = string.Format("Name {0} | Start at {1} | Maximum duration is {2} | Recur every {3}", Name, Start, MaximumDuration, Recurring);
            return value;
        }
    }
}