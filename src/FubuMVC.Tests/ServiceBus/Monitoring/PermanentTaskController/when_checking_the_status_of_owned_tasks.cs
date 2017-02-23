using System;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    
    public class when_checking_the_status_of_owned_tasks : PersistentTaskControllerContext
    {
        /*
         * check happy path
         * throws exception
         * unknown
         * an active job that isn't reflected in ownership
         * inactive
         */

        protected override void theContextIs()
        {
            settings.TakeOwnershipMessageTimeout = 2.Minutes();
            settings.TaskActivationTimeout = 2.Minutes();
            settings.TaskAvailabilityCheckTimeout = 1.Minutes();
        }

        [Fact]
        public void happy_path_checks_all_subjects()
        {
            Task("foo://1").IsFullyFunctionalAndActive();
            Task("foo://2").IsFullyFunctionalAndActive();
            Task("foo://3").IsFullyFunctionalAndActive();

            theCurrentNode.OwnedTasks = new[] {"foo://1".ToUri(), "foo://2".ToUri()};



            var task = theController.CheckStatusOfOwnedTasks();
            task.Wait();

            task.Result.Tasks.ShouldHaveTheSameElementsAs(
                new PersistentTaskStatus("foo://1".ToUri(), HealthStatus.Active),
                new PersistentTaskStatus("foo://2".ToUri(), HealthStatus.Active),
                new PersistentTaskStatus("foo://3".ToUri(), HealthStatus.Active)
                
                );
        }


        [Fact]
        public void some_tasks_fail_the_assert_available_check()
        {
            Task("foo://1").IsFullyFunctionalAndActive();
            Task("foo://2").IsActiveButNotFunctional(new DivideByZeroException());
            Task("foo://3").IsFullyFunctionalAndActive();

            theCurrentNode.OwnedTasks = new[] {"foo://1".ToUri(), "foo://2".ToUri()};

            var task = theController.CheckStatusOfOwnedTasks();
            task.Wait();

            task.Result.Tasks.ShouldHaveTheSameElementsAs(
                new PersistentTaskStatus("foo://1".ToUri(), HealthStatus.Active),
                new PersistentTaskStatus("foo://2".ToUri(), HealthStatus.Error),
                new PersistentTaskStatus("foo://3".ToUri(), HealthStatus.Active)
                
                );
        }

        [Fact]
        public void some_tasks_are_unknown()
        {
            Task("foo://1").IsFullyFunctionalAndActive();
            Task("foo://2").IsActiveButNotFunctional(new DivideByZeroException());
            //Task("foo://3").IsNotActive();

            theCurrentNode.OwnedTasks = new[] { "foo://1".ToUri(), "foo://2".ToUri(), "foo://3".ToUri() };

            var task = theController.CheckStatusOfOwnedTasks();
            task.Wait();

            task.Result.Tasks.ShouldHaveTheSameElementsAs(
                new PersistentTaskStatus("foo://1".ToUri(), HealthStatus.Active),
                new PersistentTaskStatus("foo://2".ToUri(), HealthStatus.Error),
                new PersistentTaskStatus("foo://3".ToUri(), HealthStatus.Unknown)

                );
        }

        [Fact]
        public void some_tasks_are_inactive()
        {
            Task("foo://1").IsFullyFunctionalAndActive();
            Task("foo://2").IsActiveButNotFunctional(new DivideByZeroException());
            Task("foo://3").IsNotActive();

            theCurrentNode.OwnedTasks = new[] { "foo://1".ToUri(), "foo://2".ToUri(), "foo://3".ToUri() };

            var task = theController.CheckStatusOfOwnedTasks();
            task.Wait();

            task.Result.Tasks.ShouldHaveTheSameElementsAs(
                new PersistentTaskStatus("foo://1".ToUri(), HealthStatus.Active),
                new PersistentTaskStatus("foo://2".ToUri(), HealthStatus.Error),
                new PersistentTaskStatus("foo://3".ToUri(), HealthStatus.Inactive)

                );
        }

    }
}