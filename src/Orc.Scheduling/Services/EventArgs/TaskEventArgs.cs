namespace Orc.Scheduling
{
    using System;

    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(RunningTask runningTask)
        {
            ArgumentNullException.ThrowIfNull(runningTask);

            RunningTask = runningTask;
        }

        public RunningTask RunningTask { get; private set; }
    }
}
