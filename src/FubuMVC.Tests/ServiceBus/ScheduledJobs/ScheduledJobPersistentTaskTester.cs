using System.Linq;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    [TestFixture]
    public class ScheduledJobPersistentTaskTester : InteractionContext<ScheduledJobPersistentTask>
    {
        [Test]
        public void protocol()
        {
            ClassUnderTest.Protocol.ShouldBe("scheduled");
        }

        [Test]
        public void only_permanent_task_is_its_own_uri()
        {
            ClassUnderTest.PermanentTasks()
                .Single()
                .ShouldBe(ScheduledJobPersistentTask.Uri);
        }

        [Test]
        public void creates_itself_as_the_task()
        {
            ClassUnderTest.CreateTask(ScheduledJobPersistentTask.Uri)
                .ShouldBeTheSameAs(ClassUnderTest);
        }

        [Test]
        public void assert_available_delegates_through()
        {
            ClassUnderTest.AssertAvailable();
            MockFor<IScheduledJobController>().AssertWasCalled(x => x.PerformHealthChecks());
        }

        [Test]
        public void activate_delegates()
        {
            ClassUnderTest.Activate();
            MockFor<IScheduledJobController>().AssertWasCalled(x => x.Activate());
        }

        [Test]
        public void deactivate_delegates()
        {
            ClassUnderTest.Deactivate();
            MockFor<IScheduledJobController>().AssertWasCalled(x => x.Deactivate()); 
        }
    }
}