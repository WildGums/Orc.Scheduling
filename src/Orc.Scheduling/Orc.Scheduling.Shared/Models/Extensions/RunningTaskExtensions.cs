// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTaskExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using Catel;

    public static class RunningTaskExtensions
    {
        public static bool IsExpired(this RunningTask runningTask, ITimeService timeService)
        {
            Argument.IsNotNull(() => runningTask);
            Argument.IsNotNull(() => timeService);

            var duration = timeService.CurrentDateTime - runningTask.Started;
            if (duration > runningTask.ScheduledTask.MaximumDuration)
            {
                return true;
            }

            return false;
        }
    }
}