using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    
    public class when_taking_ownership_successfully : PersistentTaskControllerContext
    {
        private const string theSubjectUriString = "good://1";
        private Task<OwnershipStatus> theTask;

        protected override void theContextIs()
        {
            Task(theSubjectUriString).IsFullyFunctional();
            Task(theSubjectUriString).Timesout = false;

            theTask = theController.TakeOwnership(theSubjectUriString.ToUri());

            settings.TakeOwnershipMessageTimeout = 2.Minutes();

            theTask.Wait();

            var timeouts = theLogger.InfoMessages.OfType<TaskActivationTimeoutFailure>();

            if (timeouts.Any())
            {
                throw new Exception("Has timeouts somehow!\n" + timeouts.Select(x => x.ToString()).Join("\n"));
            }
        }

        [Fact]
        public void should_return_OwnershipActivated()
        {
            theTask.Result.ShouldBe(OwnershipStatus.OwnershipActivated);
        }

        [Fact]
        public void activates_the_task()
        {
            Task(theSubjectUriString).IsActive.ShouldBeTrue();
        }

        [Fact]
        public void logs_the_activation()
        {
            LoggedMessageForSubject<TookOwnershipOfPersistentTask>(theSubjectUriString);
        }

        
        [Fact]
        public void persists_the_new_ownership()
        {
            theCurrentNode.OwnedTasks.ShouldContain(theSubjectUriString.ToUri());
        }
    }

    
    public class when_taking_ownership_unsuccessfully : PersistentTaskControllerContext
    {
        private const string theSubjectUriString = "bad://1";
        private Task<OwnershipStatus> theTask;

        protected override void theContextIs()
        {
            Task(theSubjectUriString).ActivationException = new DivideByZeroException();

            theTask = theController.TakeOwnership(theSubjectUriString.ToUri());

            theTask.Wait();
        }

        [Fact]
        public void should_return_Exception()
        {
            theTask.Result.ShouldBe(OwnershipStatus.Exception);
        }


        [Fact]
        public void logs_the_activation_failure()
        {
            LoggedMessageForSubject<TaskActivationFailure>(theSubjectUriString);
        }

        [Fact]
        public void does_not_persist_the_new_ownership()
        {
            theCurrentNode.OwnedTasks.ShouldNotContain(theSubjectUriString.ToUri());
        }
    }




    
    public class when_trying_to_take_ownership_of_an_already_active_task : PersistentTaskControllerContext
    {
        private const string theSubjectUriString = "good://1";
        private Task<OwnershipStatus> theTask;

        protected override void theContextIs()
        {
            Task(theSubjectUriString).IsFullyFunctionalAndActive();

            theTask = theController.TakeOwnership(theSubjectUriString.ToUri());

            theTask.Wait();
        }

        [Fact]
        public void should_return_OwnershipActivated()
        {
            theTask.Result.ShouldBe(OwnershipStatus.AlreadyOwned);
        }
    }

    
    public class when_trying_to_take_ownership_of_an_unknown_task : PersistentTaskControllerContext
    {
        [Fact]
        public void should_return_Unknown()
        {
            var task = theController.TakeOwnership("unknown://1".ToUri());
            task.Wait();
            task.Result.ShouldBe(OwnershipStatus.UnknownSubject);

        }
    }
}