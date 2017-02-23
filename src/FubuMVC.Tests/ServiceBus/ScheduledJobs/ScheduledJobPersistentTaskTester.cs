using System.Linq;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ScheduledJobs
{
    
    public class ScheduledJobPersistentTaskTester : InteractionContext<ScheduledJobPersistentTask>
    {
        [Fact]
        public void protocol()
        {
            ClassUnderTest.Protocol.ShouldBe("scheduled");
        }

        [Fact]
        public void only_permanent_task_is_its_own_uri()
        {
            ClassUnderTest.PermanentTasks()
                .Single()
                .ShouldBe(ScheduledJobPersistentTask.Uri);
        }

        [Fact]
        public void creates_itself_as_the_task()
        {
            ClassUnderTest.CreateTask(ScheduledJobPersistentTask.Uri)
                .ShouldBeTheSameAs(ClassUnderTest);
        }

        [Fact]
        public void assert_available_delegates_through()
        {
            ClassUnderTest.AssertAvailable();
            MockFor<IScheduledJobController>().AssertWasCalled(x => x.PerformHealthChecks());
        }

        [Fact]
        public void activate_delegates()
        {
            ClassUnderTest.Activate();
            MockFor<IScheduledJobController>().AssertWasCalled(x => x.Activate());
        }

        [Fact]
        public void deactivate_delegates()
        {
            ClassUnderTest.Deactivate();
            MockFor<IScheduledJobController>().AssertWasCalled(x => x.Deactivate()); 
        }
    }
}