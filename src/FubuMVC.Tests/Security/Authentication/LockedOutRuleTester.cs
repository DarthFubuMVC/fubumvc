using System;
using FubuCore.Dates;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class LockedOutRuleTester
    {
        private AuthenticationSettings theSettings;
        private SettableClock theSystemTime;
        private LockedOutRule theRule;

        [SetUp]
        public void SetUp()
        {
            theSettings = new AuthenticationSettings();
            theSystemTime = new SettableClock();

            theRule = new LockedOutRule(theSettings, theSystemTime);
        }

        [Test]
        public void is_not_locked_out_if_the_maximum_number_of_attempts_was_reached_but_the_locked_out_time_has_expired()
        {
            var request = new LoginRequest();
            request.LockedOutUntil = DateTime.Today.ToUniversalTime();

            theSystemTime.LocalNow(DateTime.Today.AddMinutes(theSettings.CooloffPeriodInMinutes + 1));

            theRule.IsLockedOut(request).ShouldBe(LoginStatus.NotAuthenticated);
        }

        [Test]
        public void is_not_locked_out_with_less_than_the_maximum_attempts()
        {
            var request = new LoginRequest();
            request.NumberOfTries = theSettings.MaximumNumberOfFailedAttempts - 1;

            theRule.IsLockedOut(request).ShouldBe(LoginStatus.NotAuthenticated);
        }

        [Test]
        public void is_locked_out_if_the_maximum_number_of_attempts_has_been_reached()
        {
            var request = new LoginRequest();
            request.NumberOfTries = theSettings.MaximumNumberOfFailedAttempts;

            theRule.IsLockedOut(request).ShouldBe(LoginStatus.LockedOut);
        }

        [Test]
        public void is_locked_out_if_the_locked_out_time_is_not_expired()
        {
            theSettings.CooloffPeriodInMinutes = 20;

            var request = new LoginRequest();
            request.NumberOfTries = theSettings.MaximumNumberOfFailedAttempts;
            request.LockedOutUntil = DateTime.Today.AddMinutes(10).ToUniversalTime();

            theSystemTime.LocalNow(DateTime.Today);

        }
    }
}