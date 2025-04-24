namespace Orc.Scheduling.Tests.Services
{
    internal class TestSchedulingService : SchedulingService
    {
        public TestSchedulingService(ITimeService timeService) 
            : base(timeService)
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
