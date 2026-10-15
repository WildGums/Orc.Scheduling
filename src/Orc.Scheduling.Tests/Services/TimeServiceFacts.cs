namespace Orc.Scheduling.Tests.Services;

using System;
using NUnit.Framework;

[TestFixture]
public class TimeServiceFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Uses_One_Minute_As_The_Default_Minute_Duration()
        {
            var timeService = new TimeService();

            Assert.That(timeService.MinuteDuration, Is.EqualTo(TimeSpan.FromMinutes(1)));
        }

        [Test]
        public void Uses_The_Configured_Minute_Duration()
        {
            var minuteDuration = TimeSpan.FromSeconds(30);
            var timeService = new TimeService(minuteDuration);

            Assert.That(timeService.MinuteDuration, Is.EqualTo(minuteDuration));
        }
    }

    [TestFixture]
    public class The_TranslateSimulatedTimeToRealTime_Method
    {
        [TestCase(60, 2, 120)]
        [TestCase(1, 2, 2)]
        [TestCase(30, 2, 60)]
        public void Converts_Simulated_Time_Using_The_Configured_Minute_Duration(
            int minuteDurationSeconds,
            int simulatedMinutes,
            int expectedRealSeconds)
        {
            var timeService = new TimeService(TimeSpan.FromSeconds(minuteDurationSeconds));

            var result = timeService.TranslateSimulatedTimeToRealTime(TimeSpan.FromMinutes(simulatedMinutes));

            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(expectedRealSeconds)));
        }
    }

    [TestFixture]
    public class The_TranslateRealTimeToSimulatedTime_Method
    {
        [TestCase(60, 120, 120)]
        [TestCase(1, 2, 120)]
        [TestCase(30, 60, 120)]
        public void Converts_Real_Time_Using_The_Configured_Minute_Duration(
            int minuteDurationSeconds,
            int realSeconds,
            int expectedSimulatedSeconds)
        {
            var timeService = new TimeService(TimeSpan.FromSeconds(minuteDurationSeconds));

            var result = timeService.TranslateRealTimeToSimulatedTime(TimeSpan.FromSeconds(realSeconds));

            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(expectedSimulatedSeconds)));
        }
    }
}