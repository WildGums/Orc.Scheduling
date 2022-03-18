// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTask.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading;
    using Catel;

    public class RunningTask
    {
        public RunningTask(IScheduledTask scheduledTask, DateTime started)
        {
            Argument.IsNotNull(() => scheduledTask);

            ScheduledTask = scheduledTask;
            Started = started;

            CancellationTokenSource = new CancellationTokenSource();
        }

        public IScheduledTask ScheduledTask { get; private set; }

        public DateTime Started { get; private set; }

#pragma warning disable IDISP006 // Implement IDisposable
        public CancellationTokenSource CancellationTokenSource { get; private set; }
#pragma warning restore IDISP006 // Implement IDisposable

        public override string ToString()
        {
            var value = string.Format("{0} | Started at {1}", ScheduledTask.Name, Started);
            return value;
        }
    }
}
