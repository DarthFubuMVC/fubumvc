using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    [TestFixture]
    public class SubscriptionCache_replace_subscriptions_tester
    {
        [Test]
        public void can_replace_the_subscriptions()
        {
            var cache = new SubscriptionCache(new ChannelGraph(), new ITransport[]{new InMemoryTransport(), });

            var subscriptions1 = new Subscription[]
            {
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription()
            };

            var subscriptions2 = new Subscription[]
            {
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription()
            };

            cache.LoadSubscriptions(subscriptions1);

            cache.ActiveSubscriptions.ShouldHaveTheSameElementsAs(subscriptions1);

            cache.LoadSubscriptions(subscriptions2);

            cache.ActiveSubscriptions.ShouldHaveTheSameElementsAs(subscriptions2);
        }

        [Test]
        public void updates_cached_routes()
        {
            var cache = new SubscriptionCache(new ChannelGraph(), new ITransport[]{new InMemoryTransport(), });

            var subscriptions1 = new[]
            {
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription()
            };
            subscriptions1[0].MessageType = typeof(string).FullName;
            subscriptions1[1].MessageType = typeof(int).FullName;

            var subscriptions2 = new[] { ObjectMother.ExistingSubscription() };
            subscriptions2[0].MessageType = typeof(int).FullName;

            cache.LoadSubscriptions(subscriptions1);
            cache.FindDestinationChannels(new Envelope { Message = "Foo" });
            cache.FindDestinationChannels(new Envelope { Message = 42 });
            cache.CachedRoutes.ContainsKey(typeof(string)).ShouldBeTrue();
            cache.CachedRoutes.ContainsKey(typeof(int)).ShouldBeTrue();

            cache.LoadSubscriptions(subscriptions2);
            cache.CachedRoutes.ContainsKey(typeof(string)).ShouldBeFalse();
            cache.CachedRoutes.ContainsKey(typeof(int)).ShouldBeTrue();
        }
    }
}