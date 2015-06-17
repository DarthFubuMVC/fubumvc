using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using FubuTransportation.Runtime;
using FubuTransportation.Subscriptions;
using NUnit.Framework;

namespace FubuTransportation.Testing.Subscriptions
{
    [TestFixture]
    public class SubscriptionCache_clearing_tester
    {
        private readonly SubscriptionCache _cache = new SubscriptionCache(
            new ChannelGraph(),
            new ITransport[] { new InMemoryTransport() });

        [Test]
        public void clears_all_subscriptions()
        {
            var subscriptions = new[]
            {
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription()
            };

            _cache.LoadSubscriptions(subscriptions);
            _cache.ActiveSubscriptions.ShouldHaveTheSameElementsAs(subscriptions);

            _cache.ClearAll();
            _cache.ActiveSubscriptions.ShouldHaveCount(0);
        }

        [Test]
        public void clears_volatile_nodes()
        {
            var envelope = ObjectMother.EnvelopeWithMessage();
            var subscription = ObjectMother.ExistingSubscription();
            subscription.MessageType = envelope.Message.GetType().FullName;

            _cache.LoadSubscriptions(new[] { subscription });
            IList<ChannelNode> channels = _cache.FindDestinationChannels(envelope).ToList();
            channels.ShouldNotBeEmpty();
            var oldNode = channels.Single();

            _cache.ClearAll();
            _cache.LoadSubscriptions(new[] { subscription });
            channels = _cache.FindDestinationChannels(envelope).ToList();
            channels.ShouldNotBeEmpty();
            channels.ShouldNotContain(oldNode);
        }
    }
}