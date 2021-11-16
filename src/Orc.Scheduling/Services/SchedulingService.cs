// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Threading;
    using Timeout = Catel.Threading.Timeout;
    using Timer = Catel.Threading.Timer;

    public class SchedulingService : ISchedulingService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITimeService _timeService;

        private readonly object _lock = new object();
        private readonly Timer _timer;
        private readonly List<IScheduledTask> _scheduledTasks = new List<IScheduledTask>();
        private readonly List<RunningTaskInfo> _runningTasks = new List<RunningTaskInfo>();
        private readonly List<CancellationToken> _cancelledTokenSources = new List<CancellationToken>();

        private bool _isUpdating;

        public SchedulingService(ITimeService timeService)
        {
            Argument.IsNotNull(() => timeService);

            _timeService = timeService;
            _timer = new Timer(OnTimerTick);

            IsEnabled = true;
        }

        public bool IsEnabled { get; private set; }

        public event EventHandler<TaskEventArgs> TaskStarted;

        public event EventHandler<TaskEventArgs> TaskCanceled;

        public event EventHandler<TaskEventArgs> TaskCompleted;

        public List<IScheduledTask> GetScheduledTasks()
        {
            lock (_lock)
            {
                return (from task in _scheduledTasks
                        select task).ToList();
            }
        }

        public List<RunningTask> GetRunningTasks()
        {
            lock (_lock)
            {
                return (from task in _runningTasks
                        select task.RunningTask).ToList();
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                if (IsEnabled)
                {
                    Log.Debug("Timer is already running, no need to start");
                    return;
                }

                Log.Debug("Starting timer");

                IsEnabled = true;

                UpdateTimerForNextEvent();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!IsEnabled)
                {
                    Log.Debug("Timer is not running, no need to stop");
                    return;
                }

                Log.Debug("Stopping timer");

                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                Log.Debug("Canceling all tasks");

                foreach (var runningTask in _runningTasks)
                {
                    TerminateTask(runningTask.RunningTask);
                }

                _runningTasks.Clear();

                IsEnabled = false;
            }
        }

        public void AddScheduledTask(IScheduledTask scheduledTask)
        {
            Argument.IsNotNull(() => scheduledTask);

            lock (_lock)
            {
                Log.Debug("Adding scheduled task {0}", scheduledTask);

                _scheduledTasks.Add(scheduledTask);

                UpdateTimerForNextEvent();
            }
        }

        public void RemoveScheduledTask(IScheduledTask scheduledTask)
        {
            Argument.IsNotNull(() => scheduledTask);

            lock (_lock)
            {
                Log.Debug("Removing scheduled task {0}", scheduledTask);

                var removedAnything = false;

                for (var i = 0; i < _scheduledTasks.Count; i++)
                {
                    if (ReferenceEquals(scheduledTask, _scheduledTasks[i]))
                    {
                        _scheduledTasks.RemoveAt(i--);
                        removedAnything = true;
                    }
                }

                if (removedAnything)
                {
                    UpdateTimerForNextEvent();
                }
            }
        }

#pragma warning disable 1998
        private async Task StartNewTasksAsync()
#pragma warning restore 1998
        {
            var tasksToStart = new List<IScheduledTask>();

            lock (_lock)
            {
                if (!IsEnabled)
                {
                    return;
                }

                for (var i = 0; i < _scheduledTasks.Count; i++)
                {
                    var scheduledTask = _scheduledTasks[i];
                    if (scheduledTask.Start <= _timeService.CurrentDateTime)
                    {
                        tasksToStart.Add(scheduledTask);
                        _scheduledTasks.RemoveAt(i--);
                    }
                }
            }

            foreach (var taskToStart in tasksToStart)
            {
                StartTask(taskToStart);
            }
        }

        private void StartTask(IScheduledTask scheduledTask)
        {
            Task task = null;
            RunningTask runningTask = null;

            lock (_lock)
            {
                if (!IsEnabled)
                {
                    return;
                }

                Log.Debug($"Starting task {scheduledTask}");

                runningTask = new RunningTask(scheduledTask, _timeService.CurrentDateTime);

#pragma warning disable 4014
                // Note: don't await, we are a scheduler
                task = TaskShim.Run(async () => await scheduledTask.InvokeAsync(), runningTask.CancellationTokenSource.Token);
                task.ContinueWith(OnRunningTaskCompleted);
#pragma warning restore 4014
                
                Log.Debug($"Started task {scheduledTask}");
            }

            if (!scheduledTask.ScheduleRecurringTaskAfterTaskExecutionHasCompleted)
            {
                // Schedule immediately, even though task is still running
                RescheduleRecurringTask(runningTask);
            }

            var completed = task.IsCompleted;
            if (completed)
            {
                // Shortcut mode
                TaskStarted?.Invoke(this, new TaskEventArgs(runningTask));

                OnRunningTaskCompleted(task);
            }
            else
            {
                _runningTasks.Add(new RunningTaskInfo(task, runningTask));

                TaskStarted?.Invoke(this, new TaskEventArgs(runningTask));
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task TerminateTasksTakingTooLongAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            lock (_lock)
            {
                for (var i = _runningTasks.Count - 1; i >= 0; i--)
                {
                    var runningTask = _runningTasks[i];
                    if (runningTask.RunningTask.IsExpired(_timeService))
                    {
                        TerminateTask(runningTask.RunningTask);
                    }
                }
            }
        }

        private void TerminateTask(RunningTask runningTask)
        {
            Log.Debug($"Terminating task {runningTask}");

            lock (_lock)
            {
                for (var i = 0; i < _runningTasks.Count; i++)
                {
                    if (ReferenceEquals(_runningTasks[i].RunningTask, runningTask))
                    {
                        _cancelledTokenSources.Add(runningTask.CancellationTokenSource.Token);
                        runningTask.CancellationTokenSource.Cancel();

                        TaskCanceled?.Invoke(this, new TaskEventArgs(runningTask));
                        return;
                    }
                }
            }
        }

        private void RescheduleRecurringTask(RunningTask runningTask)
        {
            // Note: it's important to start possible recurring tasks outside the loop
            var scheduledTask = runningTask.ScheduledTask;
            if (scheduledTask.Recurring.HasValue)
            {
                // Note: we might have overridden the interval in the clone, so we need to clone it first
                var newScheduledTask = scheduledTask.Clone();

                var startDate = _timeService.CurrentDateTime;

                if (newScheduledTask.Recurring.HasValue)
                {
                    // Use new recurring value
                    startDate = startDate.Add(newScheduledTask.Recurring.Value);
                }
                else
                {
                    // Use old recurring value
                    startDate = startDate.Add(scheduledTask.Recurring.Value);
                }

                Log.Debug($"Task {scheduledTask} is a recurring task, rescheduling a copy at '{startDate}'");

                newScheduledTask.Start = startDate;

                AddScheduledTask(newScheduledTask);
            }
        }

        private void OnRunningTaskCompleted(Task task)
        {
            RunningTask runningTask = null;
            CancellationTokenSource cancellationTokenSource = null;
            var cancellationToken = default(CancellationToken);

            var exception = task.Exception;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Task completed, searching for existing running task");
            stringBuilder.AppendLine($"  * Canceled: {task.IsCanceled}");
            stringBuilder.AppendLine($"  * Completed: {task.IsCompleted}");
            stringBuilder.AppendLine($"  * Faulted: {task.IsFaulted}");
            stringBuilder.AppendLine($"  * Exception: {exception}");

            Log.Debug(stringBuilder.ToString());

            lock (_lock)
            {
                for (var i = 0; i < _runningTasks.Count; i++)
                {
                    var possibleRunningTask = _runningTasks[i];
                    if (ReferenceEquals(possibleRunningTask.Task, task))
                    {
                        runningTask = possibleRunningTask.RunningTask;

                        _runningTasks.RemoveAt(i);

                        cancellationTokenSource = possibleRunningTask.RunningTask.CancellationTokenSource;
                        cancellationToken = cancellationTokenSource.Token;
                        cancellationTokenSource.Dispose();

                        break;
                    }
                }
            }

            if (runningTask is not null)
            {
                Log.Debug($"Found task '{runningTask}' for the completed task");

                if (runningTask.ScheduledTask.ScheduleRecurringTaskAfterTaskExecutionHasCompleted)
                {
                    RescheduleRecurringTask(runningTask);
                }

                if (!task.IsCanceled && !cancellationTokenSource.IsCancellationRequested &&
                    !_cancelledTokenSources.Contains(cancellationToken))
                {
                    TaskCompleted?.Invoke(this, new TaskEventArgs(runningTask));
                }
            }
        }

        private void UpdateTimerForNextEvent()
        {
            var now = _timeService.CurrentDateTime;
            var delta = TimeSpan.MaxValue;

            lock (_lock)
            {
                if (!IsEnabled)
                {
                    return;
                }

                Log.Debug("Calculating next timer tick");

                foreach (var scheduledTask in _scheduledTasks)
                {
                    var possibleNewDelta = scheduledTask.Start - now;
                    if (possibleNewDelta < delta)
                    {
                        delta = possibleNewDelta;
                    }
                }

                foreach (var runningTask in _runningTasks)
                {
                    var maximumDuration = runningTask.RunningTask.ScheduledTask.MaximumDuration;
                    if (maximumDuration < TimeSpan.MaxValue)
                    {
                        var endTime = runningTask.RunningTask.Started + maximumDuration;
                        var possibleNewDelta = endTime - now;
                        if (possibleNewDelta < delta)
                        {
                            delta = possibleNewDelta;
                        }
                    }
                }
            }

            if (delta <= TimeSpan.Zero)
            {
                delta = TimeSpan.Zero;
            }

            if (delta == TimeSpan.MaxValue)
            {
                Log.Debug("Disabling timer, no upcoming events");

                delta = Timeout.InfiniteTimeSpan;
            }
            else
            {
                Log.Debug($"Updating next timer tick to become active in '{delta}'");

                // We need to translate time, we might have to wait 30 minutes, but that is 30 seconds if a minute takes just 1 second
                var simulatedDelta = _timeService.TranslateSimulatedTimeToRealTime(delta);
                if (simulatedDelta <= TimeSpan.Zero)
                {
                    // Never immediately invoke timer, at least require 1 tick
                    simulatedDelta = TimeSpan.FromMilliseconds(1);
                }

                delta = simulatedDelta;
            }

            // Important to change outside the lock
            _timer.Change(delta, Timeout.InfiniteTimeSpan);
        }

#pragma warning disable AvoidAsyncVoid
        private async void OnTimerTick(object state)
#pragma warning restore AvoidAsyncVoid
        {
            lock (_lock)
            {
                if (!IsEnabled)
                {
                    return;
                }

                if (_isUpdating)
                {
                    return;
                }

                _isUpdating = true;
            }

            try
            {
                // Step 1: check for tasks that need to be canceled
                await TerminateTasksTakingTooLongAsync();

                // Step 2: check for tasks that must be started
                await StartNewTasksAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update the tasks");
            }

            lock (_lock)
            {
                _isUpdating = false;
            }

            UpdateTimerForNextEvent();
        }

        private class RunningTaskInfo
        {
            public RunningTaskInfo(Task task, RunningTask runningTask)
            {
                Task = task;
                RunningTask = runningTask;
            }

            public Task Task;
            public RunningTask RunningTask;
        }
    }
}
