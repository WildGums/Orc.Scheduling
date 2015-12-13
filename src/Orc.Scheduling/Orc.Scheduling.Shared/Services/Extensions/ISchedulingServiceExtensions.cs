// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System.Linq;
    using System.Text;
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using Catel.Text;

    public static class ISchedulingServiceExtensions
    {
        public static string GetSummary(this ISchedulingService schedulingService)
        {
            Argument.IsNotNull(() => schedulingService);

            var serviceLocator = schedulingService.GetServiceLocator();
            var languageService = serviceLocator.ResolveType<ILanguageService>();

            var scheduledTasks = (from task in schedulingService.ScheduledTasks
                                  orderby task.Start
                                  select task).ToList();

            var runningTasks = (from task in schedulingService.RunningTasks
                                orderby task.Started
                                select task).ToList();

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(languageService.GetString("Scheduling_RunningTasks"));
            stringBuilder.AppendLine("=============================");
            stringBuilder.AppendLine();

            foreach (var runningTask in runningTasks)
            {
                stringBuilder.AppendLine("* {0}", runningTask);
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(languageService.GetString("Scheduling_ScheduledTasks"));
            stringBuilder.AppendLine("=============================");
            stringBuilder.AppendLine();

            foreach (var scheduledTask in scheduledTasks)
            {
                stringBuilder.AppendLine("* {0}", scheduledTask);
            }

            return stringBuilder.ToString();
        }
    }
}