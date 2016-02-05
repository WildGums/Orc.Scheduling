// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskStartedEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using Catel;

    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(RunningTask runningTask)
        {
            Argument.IsNotNull(() => runningTask);

            RunningTask = runningTask;
        }

        public RunningTask RunningTask { get; private set; }
    }
}