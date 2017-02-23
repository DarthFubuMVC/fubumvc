using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Monitoring;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    
    public class when_trying_to_stop_a_task_that_does_not_exist : PersistentTaskControllerContext
    {
        [Fact]
        public void should_denote_failure()
        {
            var task = theController.Deactivate("nonexistent://1".ToUri());
            task.Wait();

            task.Result.ShouldBeFalse();

        }
    }

    
    public class when_stopping_a_task_successfully : PersistentTaskControllerContext
    {
        private Task<bool> theTask;

        protected override void theContextIs()
        {
            Task("running://1").IsFullyFunctionalAndActive();

            theTask = theController.Deactivate("running://1".ToUri());
            theTask.Wait();
        }

        [Fact]
        public void the_task_should_denote_success()
        {
            theTask.Result.ShouldBeTrue();
        }

        [Fact]
        public void should_stop_the_task()
        {
            Task("running://1").IsActive.ShouldBeFalse();
        }


        [Fact]
        public void the_ownership_was_removed_and_persisted()
        {
            theCurrentNode.OwnedTasks.ShouldNotContain("running://1".ToUri());
            
        }
    }

    
    public class when_stopping_a_task_unsuccessfully : PersistentTaskControllerContext
    {
        private Task<bool> theTask;

        protected override void theContextIs()
        {
            Task("running://1").IsFullyFunctionalAndActive();
            Task("running://1").DeactivateException = new DivideByZeroException();

            theTask = theController.Deactivate("running://1".ToUri());
            theTask.Wait();
        }

        [Fact]
        public void the_task_should_denote_failure_by_returning_false()
        {
            theTask.Result.ShouldBeFalse();
        }

        [Fact]
        public void logged_the_failure_message()
        {
            LoggedMessageForSubject<FailedToStopTask>("running://1");
        }

        [Fact]
        public void logged_the_exception()
        {
            theLogger.ErrorMessages.OfType<ErrorReport>()
                .Any(x => x.ExceptionText.Contains("DivideByZeroException"));
        }
    }
}