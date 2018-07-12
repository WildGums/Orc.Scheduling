// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledTask.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using Catel.Logging;

    public class ScheduledTask : ScheduledTaskBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public ScheduledTask()
        {
            MaximumDuration = TimeSpan.MaxValue;
        }

        public Func<Task> Action { get; set; }

        public override async Task InvokeAsync()
        {
            var action = Action;
            if (action == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("ScheduledTask.Action cannot be null, please provide an action to execute");
            }

            await Action();
        }

        public override IScheduledTask Clone()
        {
            return new ScheduledTask
            {
                Name = Name,
                Action = Action,
                Start = Start,
                Recurring = Recurring,
                MaximumDuration = MaximumDuration
            };
        }
    }
}