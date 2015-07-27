using System;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    [TestFixture]
    public class when_scheduling_every_day_after_specified_time : InteractionContext<EveryDayAtSpecificTime>
    {
        private DateTimeOffset nextScheduledTime;

        protected override void beforeEach()
        {
            Container.Inject(new EveryDayAtSpecificTime(hour: 06, minute: 00)); // 6am
            LocalSystemTime = DateTime.Today.AddHours(7); // 7am
            nextScheduledTime = ClassUnderTest.ScheduleNextTime(LocalSystemTime, null);
        }

        [Test]
        public void next_scheduled_time_should_start_the_next_day()
        {
            nextScheduledTime.LocalDateTime.ShouldBe(DateTime.Today.AddDays(1).AddHours(6)); // Tomorrow 6am
        }
    }

    [TestFixture]
    public class when_scheduling_every_day_before_specified_time : InteractionContext<EveryDayAtSpecificTime>
    {
        private DateTimeOffset nextScheduledTime;

        protected override void beforeEach()
        {
            Container.Inject(new EveryDayAtSpecificTime(hour: 08, minute: 00)); // 8am
            LocalSystemTime = DateTime.Today.AddHours(7); // 7am
            nextScheduledTime = ClassUnderTest.ScheduleNextTime(LocalSystemTime, null);
        }

        [Test]
        public void next_scheduled_time_should_start_the_same_day()
        {
            nextScheduledTime.LocalDateTime.ShouldBe(DateTime.Today.AddHours(8)); // Today 8am
        }
    }

    [TestFixture]
    public class when_scheduling_every_day_at_exactly_specified_time : InteractionContext<EveryDayAtSpecificTime>
    {
        private DateTimeOffset nextScheduledTime;

        protected override void beforeEach()
        {
            Container.Inject(new EveryDayAtSpecificTime(hour: 07, minute: 33)); // 7:33am
            LocalSystemTime = DateTime.Today.AddHours(7).AddMinutes(33); // 7:33am
            nextScheduledTime = ClassUnderTest.ScheduleNextTime(LocalSystemTime, null);
        }

        [Test]
        public void next_scheduled_time_should_start_the_next_day()
        {
            nextScheduledTime.LocalDateTime.ShouldBe(DateTime.Today.AddDays(1).AddHours(7).AddMinutes(33)); // Tomorrow 7:33am
        }
    }
}
