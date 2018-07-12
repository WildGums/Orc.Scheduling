// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel.Threading;
    using NUnit.Framework;

    public class SchedulingServiceExtensions
    {
        [TestFixture]
        public class TheGetSummaryMethod
        {
            [Test]
            public async Task ShowsRunningAndScheduledTasksAsync()
            {
                var timeService = new TimeService(TimeSpan.FromSeconds(1));
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

                await TaskShim.Delay(TimeSpan.FromSeconds(1));

                var summary = schedulingService.GetSummary();

                Assert.IsNotNull(summary);
            }
        }
    }
}