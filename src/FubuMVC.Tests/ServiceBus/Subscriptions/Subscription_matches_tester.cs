using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    [TestFixture]
    public class Subscription_matches_tester
    {
        [Test]
        public void fuzzy_matches()
        {
            var subscription = Subscription.For<Message1>();

            subscription.Matches(typeof(Message1)).ShouldBeTrue();
            subscription.Matches(typeof (Message2)).ShouldBeFalse();
        }
    }
}