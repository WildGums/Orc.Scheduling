// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTaskExtensionsFacts.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling.Tests.Models
{
    using System;
    using System.Threading.Tasks;
    using Catel.Threading;
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

                await TaskShim.Delay(TimeSpan.FromSeconds(2));

                Assert.IsTrue(runningTask.IsExpired(timeService));
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

                await TaskShim.Delay(TimeSpan.FromSeconds(1));

                Assert.IsFalse(runningTask.IsExpired(timeService));
            }
        }
    }
}