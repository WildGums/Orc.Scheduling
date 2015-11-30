// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTask.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading;
    using Catel;

    public class RunningTask
    {
        public RunningTask(ScheduledTask scheduledTask, DateTime started)
        {
            Argument.IsNotNull(() => scheduledTask);

            ScheduledTask = scheduledTask;
            Started = started;

            CancellationTokenSource = new CancellationTokenSource();
        }

        public ScheduledTask ScheduledTask { get; private set; }

        public DateTime Started { get; private set; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}