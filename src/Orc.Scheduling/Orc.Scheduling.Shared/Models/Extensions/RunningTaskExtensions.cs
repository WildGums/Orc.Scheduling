// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTaskExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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