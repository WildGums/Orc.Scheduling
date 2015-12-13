// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskStartedEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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