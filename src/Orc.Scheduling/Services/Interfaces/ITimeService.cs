namespace Orc.Scheduling;

using System;
using System.Threading.Tasks;

public interface ITimeService
{
    TimeSpan MinuteDuration { get; }
    DateTimeOffset CurrentDateTime { get; }

    Task WaitAsync(TimeSpan timeSpan);

    TimeSpan TranslateSimulatedTimeToRealTime(TimeSpan timeSpan);

    TimeSpan TranslateRealTimeToSimulatedTime(TimeSpan timePassed);
}
