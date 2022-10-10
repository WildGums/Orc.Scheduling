namespace Orc.Scheduling
{
    using System;
    using Catel;

    public static class ITimeServiceExtensions
    {
        public static TimeSpan TranslateSimulatedTimeToRealTime(this ITimeService timeService, TimeSpan timeSpan)
        {
            ArgumentNullException.ThrowIfNull(timeService);

            var realTimeToWaitInMinutes = timeSpan.TotalMinutes * timeService.MinuteDuration.TotalMinutes;
            var realTimeToWait = TimeSpan.FromMinutes(realTimeToWaitInMinutes);

            return realTimeToWait;
        }

        public static TimeSpan TranslateRealTimeToSimulatedTime(this ITimeService timeService, TimeSpan timePassed)
        {
            ArgumentNullException.ThrowIfNull(timeService);

            // Note: time passed is always simulation mode, so we need to get the actual multiplier
            var multiplier = TimeSpan.FromMinutes(1).TotalSeconds/timeService.MinuteDuration.TotalSeconds;
            var simTimeInSeconds = timePassed.TotalSeconds*multiplier;
            var simTimeToWait = TimeSpan.FromSeconds(simTimeInSeconds);

            return simTimeToWait;
        }
    }
}
