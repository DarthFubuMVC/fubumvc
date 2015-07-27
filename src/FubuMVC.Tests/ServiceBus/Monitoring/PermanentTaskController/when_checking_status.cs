using System;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    [TestFixture]
    public class when_checking_status : PersistentTaskControllerContext
    {
        protected override void theContextIs()
        {
            Task("running://1").IsFullyFunctionalAndActive();
            Task("stopped://1").IsActive = false;
            Task("error://1").IsActiveButNotFunctional(new DivideByZeroException());
            Task("timeout://1").IsFullyFunctionalAndActive();
            Task("timeout://1").Timesout = true;

            settings.TakeOwnershipMessageTimeout = 45.Seconds();
            settings.TaskAvailabilityCheckTimeout = 45.Seconds();
        }

        private HealthStatus findStatus(string uriString)
        {
            var uri = uriString.ToUri();
            var task = theController.CheckStatus(uri);
            task.Wait();

            return task.Result;
        }

        [Test]
        public void check_the_status_of_a_task_that_times_out_in_its_availibility_check()
        {
            settings.TakeOwnershipMessageTimeout = 2.Seconds();
            settings.TaskAvailabilityCheckTimeout = 2.Seconds();

            findStatus("timeout://1").ShouldBe(HealthStatus.Timedout);
        }

        [Test]
        public void check_the_status_of_an_unknown_task()
        {
            findStatus("unknown://1").ShouldBe(HealthStatus.Unknown);
        }

        [Test]
        public void check_the_status_of_an_active_and_functional_task()
        {
            findStatus("running://1").ShouldBe(HealthStatus.Active);
        }

        [Test]
        public void check_the_status_of_an_inactive_task()
        {
            findStatus("stopped://1").ShouldBe(HealthStatus.Inactive);
        }

        [Test]
        public void check_the_status_of_an_active_task_that_has_errored_out()
        {
            findStatus("error://1").ShouldBe(HealthStatus.Error);
        }
    }
}