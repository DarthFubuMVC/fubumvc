using FubuMVC.Core.ServiceBus.Scheduling;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Scheduling
{
    
    public class ThreadSchedulerTester
    {
        [Fact]
        public void can_schedule_work()
        {
            var ran = false;
            using(var scheduler = ThreadScheduler.Default())
            {
                scheduler.Start(() => ran = true);
                Wait.Until(() => ran).ShouldBeTrue();
            }
        }

        [Fact]
        public void can_use_multiple_threads()
        {
            using (var scheduler = new ThreadScheduler(5))
            {
                scheduler.Start(() => { });
                scheduler.Threads.ShouldHaveCount(5);
            }
        }

        [Fact]
        public void unstarted_threads_should_be_empty()
        {
            using (var scheduler = new ThreadScheduler(5))
            {
                scheduler.Threads.ShouldHaveCount(0);
            }
        }
    }
}