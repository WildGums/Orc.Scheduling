namespace Orc.Scheduling.Tests.Models;

using System;
using System.Threading.Tasks;
using NUnit.Framework;

public class RunningTaskExtensionsFacts
{
    [TestFixture]
    public class TheIsExpiredMethod
    {
        [TestCase]
        public async Task ReturnsTrueForExpiredTaskAsync()
        {
            var timeService = new TimeService(TimeSpan.FromSeconds(1));

            var scheduledTask = new ScheduledTask
            {
                MaximumDuration = TimeSpan.FromMinutes(1)
            };

            var runningTask = new RunningTask(scheduledTask, DateTime.Now);

            await Task.Delay(TimeSpan.FromSeconds(2));

            Assert.That(runningTask.IsExpired(timeService), Is.True);
        }

        [TestCase]
        public async Task ReturnsFalseForNonExpiredTaskAsync()
        {
            var timeService = new TimeService(TimeSpan.FromSeconds(1));

            var scheduledTask = new ScheduledTask
            {
                MaximumDuration = TimeSpan.FromMinutes(2)
            };

            var runningTask = new RunningTask(scheduledTask, DateTime.Now);

            await Task.Delay(TimeSpan.FromSeconds(1));

            Assert.That(runningTask.IsExpired(timeService), Is.False);
        }
    }
}
