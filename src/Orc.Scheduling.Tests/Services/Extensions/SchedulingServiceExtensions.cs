namespace Orc.Scheduling.Tests.Services;

using System;
using System.Threading.Tasks;
using Catel.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

public class SchedulingServiceExtensions
{
    [TestFixture]
    public class TheGetSummaryMethod
    {
        [Test]
        public async Task Shows_Running_And_Scheduled_Tasks()
        {
            var timeService = new TimeService(TimeSpan.FromSeconds(1));
            var schedulingService = new SchedulingService(NullLogger<SchedulingService>.Instance, timeService);

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

            await Task.Delay(TimeSpan.FromSeconds(1));

            var languageServiceMock = new Mock<ILanguageService>();
            languageServiceMock.Setup(x => x.GetString(It.IsAny<string>()))
                .Returns((string s) => s);

            var summary = schedulingService.GetSummary(languageServiceMock.Object);

            Assert.That(summary, Is.Not.Null);
        }
    }
}
