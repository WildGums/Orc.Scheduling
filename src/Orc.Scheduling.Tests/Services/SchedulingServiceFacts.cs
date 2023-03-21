namespace Orc.Scheduling.Tests.Services;

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Catel.Threading;

[TestFixture]
public class SchedulingServiceFacts
{
    [Test]
    public async Task CompletesTasksAfterSpecificPeriodAsync()
    {
        var timeService = new TimeService(TimeSpan.FromSeconds(1));
        var schedulingService = new SchedulingService(timeService);

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddHours(5)
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        bool isTaskCompleted = false;
        var scheduledTask2 = new ScheduledTask
        {
            Name = "task 2",
            Start = timeService.CurrentDateTime,
            Action = async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                isTaskCompleted = true;
            }
        };

        schedulingService.AddScheduledTask(scheduledTask2);

        var isCanceled = false;
        schedulingService.TaskCanceled += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCanceled = true;
            }
        };

        var isCompleted = false;
        schedulingService.TaskCompleted += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCompleted = true;
            }
        };

        await Task.Delay(TimeSpan.FromSeconds(5));

        schedulingService.Stop();

        // Additional wait time to allow canceling etc
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.IsTrue(isCompleted);
        Assert.IsTrue(isTaskCompleted);
        Assert.IsFalse(isCanceled);
    }

    [Test]
    public async Task RestartsRecurringTasksAsync()
    {
        // Note: this is a real-time service! Don't wait for minutes here, otherwise unit tests will take too long ;-)
        var timeService = new TimeService(TimeSpan.FromMinutes(1));
        var schedulingService = new SchedulingService(timeService);

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddHours(5)
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        var taskCompletedCounter = 0;
        var scheduledTask2 = new ScheduledTask
        {
            Name = "task 2",
            Start = timeService.CurrentDateTime,
            Action = () =>
            {
                taskCompletedCounter++;
                return Task.CompletedTask;
            },
            Recurring = TimeSpan.FromSeconds(2)
        };

        schedulingService.AddScheduledTask(scheduledTask2);

        var isCanceled = false;
        schedulingService.TaskCanceled += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCanceled = true;
            }
        };

        var isCompleted = false;
        schedulingService.TaskCompleted += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCompleted = true;
            }
        };

        await Task.Delay(TimeSpan.FromSeconds(5));

        schedulingService.Stop();

        Assert.IsTrue(isCompleted);
        Assert.IsFalse(isCanceled);

        Assert.AreEqual(3, taskCompletedCounter);
    }

    [Test]
    public async Task CancelsRunningTasksAboveMaximumDurationAsync()
    {
        var timeService = new TimeService(TimeSpan.FromSeconds(1));
        var schedulingService = new SchedulingService(timeService);

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddHours(5)
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        bool isTaskCompleted = false;
        var scheduledTask2 = new ScheduledTask
        {
            Name = "task 2",
            Start = timeService.CurrentDateTime,
            Action = async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                isTaskCompleted = true;
            },
            MaximumDuration = TimeSpan.FromSeconds(30)
        };

        schedulingService.AddScheduledTask(scheduledTask2);

        var isCanceled = false;
        schedulingService.TaskCanceled += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCanceled = true;
            }
        };

        var isCompleted = false;
        schedulingService.TaskCompleted += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCompleted = true;
            }
        };

        await Task.Delay(TimeSpan.FromSeconds(2));

        Assert.IsFalse(isCompleted);
        Assert.IsFalse(isTaskCompleted);
        Assert.IsTrue(isCanceled);

        schedulingService.Stop();
    }

    [Test]
    public async Task CancelsRunningTasksWhenStopIsInvokedAsync()
    {
        var timeService = new TimeService(TimeSpan.FromSeconds(1));
        var schedulingService = new SchedulingService(timeService);

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddHours(5)
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        bool isTaskCompleted = false;
        var scheduledTask2 = new ScheduledTask
        {
            Name = "task 2",
            Start = timeService.CurrentDateTime,
            Action = async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                isTaskCompleted = true;
            }
        };

        schedulingService.AddScheduledTask(scheduledTask2);

        var isCanceled = false;
        schedulingService.TaskCanceled += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCanceled = true;
            }
        };

        var isCompleted = false;
        schedulingService.TaskCompleted += (sender, e) =>
        {
            if (ReferenceEquals(e.RunningTask.ScheduledTask, scheduledTask2))
            {
                isCompleted = true;
            }
        };

        await Task.Delay(TimeSpan.FromSeconds(2));

        schedulingService.Stop();

        Assert.IsFalse(isCompleted);
        Assert.IsFalse(isTaskCompleted);
        Assert.IsTrue(isCanceled);
    }

    [Test]
    public async Task RemovesScheduledTasksAsync()
    {
        var timeService = new TimeService(TimeSpan.FromMinutes(1));
        var schedulingService = new SchedulingService(timeService);

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddHours(5)
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        var scheduledTask2 = new ScheduledTask
        {
            Name = "task 2",
            Start = timeService.CurrentDateTime,
            Action = async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        };

        schedulingService.AddScheduledTask(scheduledTask2);

        Assert.AreEqual(2, schedulingService.GetScheduledTasks().Count);

        schedulingService.RemoveScheduledTask(scheduledTask2);

        Assert.AreEqual(1, schedulingService.GetScheduledTasks().Count);
    }

    [Test]
    public async Task DisablesTimerWhenNoTasksAreScheduledAsync()
    {
        var timeService = new TimeService(TimeSpan.FromMinutes(1));
        var schedulingService = new SchedulingService(timeService);

        schedulingService.Start();
    }

    [Test]
    public async Task RespectsReschedulingAfterExecutionCompletedIsFalseAsync()
    {
        // Note: this is a real-time service! Don't wait for minutes here, otherwise unit tests will take too long ;-)
        var timeService = new TimeService(TimeSpan.FromSeconds(1));
        var schedulingService = new SchedulingService(timeService);
        var hasReceivedCompletedEvent = false;

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddMinutes(1),
            Recurring = TimeSpan.FromHours(1),
            ScheduleRecurringTaskAfterTaskExecutionHasCompleted = false,
            Action = async () => { await Task.Delay(1000); }
        };

        schedulingService.TaskStarted += (sender, e) =>
        {
            // Task must be here
            var newlyScheduledTask = schedulingService.GetScheduledTasks().FirstOrDefault();

            Assert.IsNotNull(newlyScheduledTask);

            hasReceivedCompletedEvent = true;
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        schedulingService.Start();

        await Task.Delay(10 * 1000);

        schedulingService.Stop();

        Assert.IsTrue(hasReceivedCompletedEvent);
    }

    [Test]
    public async Task RespectsReschedulingAfterExecutionCompletedIsTrueAsync()
    {
        // Note: this is a real-time service! Don't wait for minutes here, otherwise unit tests will take too long ;-)
        var timeService = new TimeService(TimeSpan.FromSeconds(1));
        var schedulingService = new SchedulingService(timeService);
        var hasReceivedCompletedEvent = false;

        var scheduledTask1 = new ScheduledTask
        {
            Name = "task 1",
            Start = timeService.CurrentDateTime.AddMinutes(1),
            Recurring = TimeSpan.FromHours(1),
            ScheduleRecurringTaskAfterTaskExecutionHasCompleted = true,
            Action = async () => { await Task.Delay(1000); }
        };

        schedulingService.TaskStarted += (sender, e) =>
        {
            // Task must be *not* here
            var newlyScheduledTask = schedulingService.GetScheduledTasks().FirstOrDefault();

            Assert.IsNull(newlyScheduledTask);
        };

        schedulingService.TaskCompleted += (sender, e) =>
        {
            // Task must be here
            var newlyScheduledTask = schedulingService.GetScheduledTasks().FirstOrDefault();

            Assert.IsNotNull(newlyScheduledTask);

            hasReceivedCompletedEvent = true;
        };

        schedulingService.AddScheduledTask(scheduledTask1);

        schedulingService.Start();

        await Task.Delay(10 * 1000);

        schedulingService.Stop();

        Assert.IsTrue(hasReceivedCompletedEvent);
    }
}
