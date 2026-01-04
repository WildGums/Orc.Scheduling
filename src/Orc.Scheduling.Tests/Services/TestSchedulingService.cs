namespace Orc.Scheduling.Tests.Services
{
    using Microsoft.Extensions.Logging.Abstractions;

    internal class TestSchedulingService : SchedulingService
    {
        public TestSchedulingService(ITimeService timeService) 
            : base(NullLogger<SchedulingService>.Instance, timeService)
        {
        }

        public int UpdateTimerCounter { get; private set; }

        internal override void UpdateTimerForNextEvent()
        {
            UpdateTimerCounter++;

            base.UpdateTimerForNextEvent();
        }
    }
}
