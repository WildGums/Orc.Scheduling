namespace Orc.Scheduling;

using System;
using System.Linq;
using System.Text;
using Catel.IoC;
using Catel.Services;
using Catel.Text;

public static class ISchedulingServiceExtensions
{
    public static string GetSummary(this ISchedulingService schedulingService)
    {
        ArgumentNullException.ThrowIfNull(schedulingService);

#pragma warning disable IDISP001 // Dispose created
        var serviceLocator = schedulingService.GetServiceLocator();
#pragma warning restore IDISP001 // Dispose created
        var languageService = serviceLocator.ResolveRequiredType<ILanguageService>();

        var scheduledTasks = (from task in schedulingService.GetScheduledTasks()
            orderby task.Start
            select task).ToList();

        var runningTasks = (from task in schedulingService.GetRunningTasks()
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
