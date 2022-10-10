namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;

    public interface ITimeService
    {
        TimeSpan MinuteDuration { get; }
        DateTime CurrentDateTime { get; }

        Task WaitAsync(TimeSpan timeSpan);
    }
}
