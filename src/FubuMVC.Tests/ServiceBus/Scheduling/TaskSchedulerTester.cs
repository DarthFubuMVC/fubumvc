using FubuMVC.Core.ServiceBus.Scheduling;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Scheduling
{
    public class TaskSchedulerTester
    {
        [Test]
        public void can_schedule_work()
        {
            var ran = false;
            using (var scheduler = TaskScheduler.Default())
            {
                scheduler.Start(() => ran = true);
                Wait.Until(() => ran).ShouldBeTrue();
            }
        }

        [Test]
        public void can_use_multiple_tasks()
        {
            using (var scheduler = new TaskScheduler(5))
            {
                scheduler.Start(() => { });
                scheduler.Tasks.ShouldHaveCount(5);
            }
        }

        [Test]
        public void unstarted_tasks_should_be_empty()
        {
            using (var scheduler = new TaskScheduler(5))
            {
                scheduler.Tasks.ShouldHaveCount(0);
            }
        }
    }
}