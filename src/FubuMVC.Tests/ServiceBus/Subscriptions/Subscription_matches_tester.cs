using FubuMVC.Core.ServiceBus.Subscriptions;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    
    public class Subscription_matches_tester
    {
        [Fact]
        public void fuzzy_matches()
        {
            var subscription = Subscription.For<Message1>();

            subscription.Matches(typeof(Message1)).ShouldBeTrue();
            subscription.Matches(typeof (Message2)).ShouldBeFalse();
        }
    }
}