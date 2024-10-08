﻿namespace Orc.Scheduling;

using System;
using System.Threading.Tasks;
using Catel.Logging;

public class TimeService : ITimeService
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly DateTime _start;
    private readonly DateTime _actualStart;

    public TimeService()
        : this(TimeSpan.FromMinutes(1))
    {
    }

    public TimeService(TimeSpan minuteDuration)
        : this(minuteDuration, DateTime.Now)
    {
    }

    public TimeService(TimeSpan minuteDuration, DateTime start)
    {
        Log.Debug("Creating time service where a minute lasts for '{0}' seconds, start date/time is '{1}'", minuteDuration.TotalSeconds, start);

        _start = start;
        _actualStart = DateTime.Now;
        MinuteDuration = minuteDuration;
    }

    public DateTime CurrentDateTime
    {
        get
        {
            var delta = DateTime.Now - _actualStart;
            var simulatedDelta = this.TranslateRealTimeToSimulatedTime(delta);

            return _start.Add(simulatedDelta);
        }
    }

    public TimeSpan MinuteDuration { get; private set; }

    public Task WaitAsync(TimeSpan timeSpan)
    {
        var realTimeToWait = this.TranslateSimulatedTimeToRealTime(timeSpan);

        return Task.Delay(realTimeToWait);
    }
}
