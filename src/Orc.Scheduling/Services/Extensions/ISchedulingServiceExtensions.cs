namespace Orc.Scheduling;

using System.Linq;
using System.Text;
using Catel.Services;
using Catel.Text;

public static class ISchedulingServiceExtensions
{
    public static string GetSummary(this ISchedulingService schedulingService, ILanguageService languageService)
    {
        var scheduledTasks = (from task in schedulingService.GetScheduledTasks()
            orderby task.Start
            select task).ToList();

        var runningTasks = (from task in schedulingService.GetRunningTasks()
            orderby task.Started
            select task).ToList();

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(languageService.GetRequiredString("Scheduling_RunningTasks"));
        stringBuilder.AppendLine(languageService.GetRequiredString("Scheduling_Separator"));
        stringBuilder.AppendLine();

        foreach (var runningTask in runningTasks)
        {
            stringBuilder.AppendLine(languageService.GetRequiredString("Scheduling_TaskItem"), runningTask);
        }

        stringBuilder.AppendLine();

        stringBuilder.AppendLine(languageService.GetRequiredString("Scheduling_ScheduledTasks"));
        stringBuilder.AppendLine(languageService.GetRequiredString("Scheduling_Separator"));
        stringBuilder.AppendLine();

        foreach (var scheduledTask in scheduledTasks)
        {
            stringBuilder.AppendLine(languageService.GetRequiredString("Scheduling_TaskItem"), scheduledTask);
        }

        return stringBuilder.ToString();
    }
}
