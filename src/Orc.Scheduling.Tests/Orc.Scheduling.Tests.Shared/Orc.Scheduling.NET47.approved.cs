﻿[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.7", FrameworkDisplayName=".NET Framework 4.7")]


public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.Scheduling
{
    
    public interface IScheduledTask : System.ICloneable
    {
        System.TimeSpan MaximumDuration { get; set; }
        string Name { get; set; }
        System.Nullable<System.TimeSpan> Recurring { get; set; }
        bool ScheduleRecurringTaskAfterTaskExecutionHasCompleted { get; set; }
        System.DateTime Start { get; set; }
        System.Threading.Tasks.Task InvokeAsync();
    }
    public interface ISchedulingService
    {
        bool IsEnabled { get; }
        System.Collections.Generic.List<Orc.Scheduling.RunningTask> RunningTasks { get; }
        System.Collections.Generic.List<Orc.Scheduling.IScheduledTask> ScheduledTasks { get; }
        public event System.EventHandler<Orc.Scheduling.TaskEventArgs> TaskCanceled;
        public event System.EventHandler<Orc.Scheduling.TaskEventArgs> TaskCompleted;
        public event System.EventHandler<Orc.Scheduling.TaskEventArgs> TaskStarted;
        void AddScheduledTask(Orc.Scheduling.IScheduledTask scheduledTask);
        void RemoveScheduledTask(Orc.Scheduling.IScheduledTask scheduledTask);
        void Start();
        void Stop();
    }
    public class static ISchedulingServiceExtensions
    {
        public static string GetSummary(this Orc.Scheduling.ISchedulingService schedulingService) { }
    }
    public interface ITimeService
    {
        System.DateTime CurrentDateTime { get; }
        System.TimeSpan MinuteDuration { get; }
        System.Threading.Tasks.Task WaitAsync(System.TimeSpan timeSpan);
    }
    public class static ITimeServiceExtensions
    {
        public static System.TimeSpan TranslateRealTimeToSimulatedTime(this Orc.Scheduling.ITimeService timeService, System.TimeSpan timePassed) { }
        public static System.TimeSpan TranslateSimulatedTimeToRealTime(this Orc.Scheduling.ITimeService timeService, System.TimeSpan timeSpan) { }
    }
    public class RunningTask
    {
        public RunningTask(Orc.Scheduling.IScheduledTask scheduledTask, System.DateTime started) { }
        public System.Threading.CancellationTokenSource CancellationTokenSource { get; }
        public Orc.Scheduling.IScheduledTask ScheduledTask { get; }
        public System.DateTime Started { get; }
        public override string ToString() { }
    }
    public class static RunningTaskExtensions
    {
        public static bool IsExpired(this Orc.Scheduling.RunningTask runningTask, Orc.Scheduling.ITimeService timeService) { }
    }
    public class ScheduledTask : Orc.Scheduling.ScheduledTaskBase
    {
        public ScheduledTask() { }
        public System.Func<System.Threading.Tasks.Task> Action { get; set; }
        public override object Clone() { }
        public override System.Threading.Tasks.Task InvokeAsync() { }
    }
    public abstract class ScheduledTaskBase : Orc.Scheduling.IScheduledTask, System.ICloneable
    {
        protected ScheduledTaskBase() { }
        public System.TimeSpan MaximumDuration { get; set; }
        public string Name { get; set; }
        public System.Nullable<System.TimeSpan> Recurring { get; set; }
        public bool ScheduleRecurringTaskAfterTaskExecutionHasCompleted { get; set; }
        public System.DateTime Start { get; set; }
        public abstract object Clone();
        public abstract System.Threading.Tasks.Task InvokeAsync();
        public override string ToString() { }
    }
    public class SchedulingService : Orc.Scheduling.ISchedulingService
    {
        public SchedulingService(Orc.Scheduling.ITimeService timeService) { }
        public bool IsEnabled { get; }
        public System.Collections.Generic.List<Orc.Scheduling.RunningTask> RunningTasks { get; }
        public System.Collections.Generic.List<Orc.Scheduling.IScheduledTask> ScheduledTasks { get; }
        public event System.EventHandler<Orc.Scheduling.TaskEventArgs> TaskCanceled;
        public event System.EventHandler<Orc.Scheduling.TaskEventArgs> TaskCompleted;
        public event System.EventHandler<Orc.Scheduling.TaskEventArgs> TaskStarted;
        public void AddScheduledTask(Orc.Scheduling.IScheduledTask scheduledTask) { }
        public void RemoveScheduledTask(Orc.Scheduling.IScheduledTask scheduledTask) { }
        public void Start() { }
        public void Stop() { }
    }
    public class TaskEventArgs : System.EventArgs
    {
        public TaskEventArgs(Orc.Scheduling.RunningTask runningTask) { }
        public Orc.Scheduling.RunningTask RunningTask { get; }
    }
    public class TimeService : Orc.Scheduling.ITimeService
    {
        public TimeService() { }
        public TimeService(System.TimeSpan minuteDuration) { }
        public TimeService(System.TimeSpan minuteDuration, System.DateTime start) { }
        public System.DateTime CurrentDateTime { get; }
        public System.TimeSpan MinuteDuration { get; }
        public System.Threading.Tasks.Task WaitAsync(System.TimeSpan timeSpan) { }
    }
}