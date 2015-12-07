// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingServiceFacts.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Catel.Threading;

    [TestFixture]
    public class SchedulingServiceFacts
    {
        [TestCase]
        public async Task CompletesTasksAfterSpecificPeriod()
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
                    await TaskShim.Delay(TimeSpan.FromSeconds(2));
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

            await TaskShim.Delay(TimeSpan.FromSeconds(5));

            schedulingService.Stop();

            // Additional wait time to allow canceling etc
            await TaskShim.Delay(TimeSpan.FromSeconds(1));

            Assert.IsTrue(isCompleted);
            Assert.IsTrue(isTaskCompleted);
            Assert.IsFalse(isCanceled);
        }

        [TestCase]
        public async Task RestartsRecurringTasks()
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
                    return TaskHelper.Completed;
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

            await TaskShim.Delay(TimeSpan.FromSeconds(5));

            schedulingService.Stop();

            Assert.IsTrue(isCompleted);
            Assert.IsFalse(isCanceled);

            Assert.AreEqual(3, taskCompletedCounter);
        }

        [TestCase]
        public async Task CancelsRunningTasksAboveMaximumDuration()
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
                    await TaskShim.Delay(TimeSpan.FromMinutes(1));
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

            await TaskShim.Delay(TimeSpan.FromSeconds(2));

            Assert.IsFalse(isCompleted);
            Assert.IsFalse(isTaskCompleted);
            Assert.IsTrue(isCanceled);

            schedulingService.Stop();
        }

        [TestCase]
        public async Task CancelsRunningTasksWhenStopIsInvoked()
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
                    await TaskShim.Delay(TimeSpan.FromMinutes(1));
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

            await TaskShim.Delay(TimeSpan.FromSeconds(2));

            schedulingService.Stop();

            Assert.IsFalse(isCompleted);
            Assert.IsFalse(isTaskCompleted);
            Assert.IsTrue(isCanceled);
        }

        [TestCase]
        public async Task RemovesScheduledTasks()
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
                    await TaskShim.Delay(TimeSpan.FromMinutes(1));
                }
            };

            schedulingService.AddScheduledTask(scheduledTask2);

            Assert.AreEqual(2, schedulingService.ScheduledTasks.Count);

            schedulingService.RemoveScheduledTask(scheduledTask2);

            Assert.AreEqual(1, schedulingService.ScheduledTasks.Count);
        }

        [TestCase]
        public async Task DisablesTimerWhenNoTasksAreScheduled()
        {
            var timeService = new TimeService(TimeSpan.FromMinutes(1));
            var schedulingService = new SchedulingService(timeService);

            schedulingService.Start();
        }
    }
}