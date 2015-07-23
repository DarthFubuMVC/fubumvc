using System;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Services.Messaging
{
    [TestFixture]
    public class WaitTester
    {
        [Test]
        public void Wait_until_should_return_true_immediately()
        {
            Wait.Until(() => true)
                .ShouldBeTrue();
        }
        
        [Test]
        public void Wait_until_should_return_false_immediately()
        {
            Wait.Until(() => false)
                .ShouldBeFalse();
        }

        [Test]
        public void can_really_wait_until_the_condition_passes_in_subsequent_calls()
        {
            int i = 0;

            Func<bool> condition = () => {
                i++;
                return i > 3;
            };

            Wait.Until(condition).ShouldBeTrue();
        }
    }
}