namespace Orc.Scheduling;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class TimeService : ITimeService
{
    private static readonly double TotalSecondsInMinute = TimeSpan.FromMinutes(1).TotalSeconds;

    private static readonly ILogger Logger = LogManager.GetLogger(typeof(TimeService));

    private readonly DateTimeOffset _start;
    private readonly DateTimeOffset _actualStart;

    public TimeService()
        : this(TimeSpan.FromMinutes(1))
    {
    }

    public TimeService(TimeSpan minuteDuration)
        : this(minuteDuration, DateTimeOffset.Now)
    {
    }

    public TimeService(TimeSpan minuteDuration, DateTimeOffset start)
    {
        Logger.LogDebug("Creating time service where a minute lasts for '{0}' seconds, start date/time is '{1}'", minuteDuration.TotalSeconds, start);

        _start = start;
        _actualStart = DateTimeOffset.Now;
        MinuteDuration = minuteDuration;
    }

    public DateTimeOffset CurrentDateTime
    {
        get
        {
            var delta = DateTimeOffset.Now - _actualStart;
            var simulatedDelta = TranslateRealTimeToSimulatedTime(delta);

            return _start.Add(simulatedDelta);
        }
    }

    public TimeSpan MinuteDuration { get; private set; }

    public virtual Task WaitAsync(TimeSpan timeSpan)
    {
        var realTimeToWait = TranslateSimulatedTimeToRealTime(timeSpan);

        return Task.Delay(realTimeToWait);
    }

    public virtual TimeSpan TranslateSimulatedTimeToRealTime(TimeSpan timeSpan)
    {
        var realTimeToWaitInMinutes = timeSpan.TotalMinutes * MinuteDuration.TotalMinutes;
        var realTimeToWait = TimeSpan.FromMinutes(realTimeToWaitInMinutes);

        return realTimeToWait;
    }

    public virtual TimeSpan TranslateRealTimeToSimulatedTime(TimeSpan timePassed)
    {
        // Note: time passed is always simulation mode, so we need to get the actual multiplier
        var multiplier = TotalSecondsInMinute / MinuteDuration.TotalSeconds;
        var simTimeInSeconds = timePassed.TotalSeconds * multiplier;
        var simTimeToWait = TimeSpan.FromSeconds(simTimeInSeconds);

        return simTimeToWait;
    }
}
