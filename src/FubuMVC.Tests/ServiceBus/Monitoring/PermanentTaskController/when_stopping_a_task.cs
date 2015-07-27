using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    [TestFixture]
    public class when_trying_to_stop_a_task_that_does_not_exist : PersistentTaskControllerContext
    {
        [Test]
        public void should_denote_failure()
        {
            var task = theController.Deactivate("nonexistent://1".ToUri());
            task.Wait();

            task.Result.ShouldBeFalse();

        }
    }

    [TestFixture]
    public class when_stopping_a_task_successfully : PersistentTaskControllerContext
    {
        private Task<bool> theTask;

        protected override void theContextIs()
        {
            Task("running://1").IsFullyFunctionalAndActive();

            theTask = theController.Deactivate("running://1".ToUri());
            theTask.Wait();
        }

        [Test]
        public void the_task_should_denote_success()
        {
            theTask.Result.ShouldBeTrue();
        }

        [Test]
        public void should_stop_the_task()
        {
            Task("running://1").IsActive.ShouldBeFalse();
        }


        [Test]
        public void the_ownership_was_removed_and_persisted()
        {
            theCurrentNode.OwnedTasks.ShouldNotContain("running://1".ToUri());
            
        }
    }

    [TestFixture]
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

        [Test]
        public void the_task_should_denote_failure_by_returning_false()
        {
            theTask.Result.ShouldBeFalse();
        }

        [Test]
        public void logged_the_failure_message()
        {
            LoggedMessageForSubject<FailedToStopTask>("running://1");
        }

        [Test]
        public void logged_the_exception()
        {
            theLogger.ErrorMessages.OfType<ErrorReport>()
                .Any(x => x.ExceptionText.Contains("DivideByZeroException"));
        }
    }
}