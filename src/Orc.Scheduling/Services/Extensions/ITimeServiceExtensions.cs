// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimeServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using Catel;

    public static class ITimeServiceExtensions
    {
        #region Methods
        public static TimeSpan TranslateSimulatedTimeToRealTime(this ITimeService timeService, TimeSpan timeSpan)
        {
            Argument.IsNotNull(() => timeService);

            var realTimeToWaitInMinutes = timeSpan.TotalMinutes * timeService.MinuteDuration.TotalMinutes;
            var realTimeToWait = TimeSpan.FromMinutes(realTimeToWaitInMinutes);

            return realTimeToWait;
        }

        public static TimeSpan TranslateRealTimeToSimulatedTime(this ITimeService timeService, TimeSpan timePassed)
        {
            Argument.IsNotNull(() => timeService);

            // Note: time passed is always simulation mode, so we need to get the actual multiplier
            var multiplier = TimeSpan.FromMinutes(1).TotalSeconds/timeService.MinuteDuration.TotalSeconds;
            var simTimeInSeconds = timePassed.TotalSeconds*multiplier;
            var simTimeToWait = TimeSpan.FromSeconds(simTimeInSeconds);

            return simTimeToWait;
        }
        #endregion
    }
}