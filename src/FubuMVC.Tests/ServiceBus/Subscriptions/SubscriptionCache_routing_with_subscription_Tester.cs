using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    [TestFixture]
    public class SubscriptionCache_routing_with_subscription_Tester
    {
        public readonly SubscriptionSettings theSettings = InMemoryTransport.ToInMemory<SubscriptionSettings>();
        private FubuRuntime _runtime;
        private SubscriptionCache theCache;

        [SetUp]
        public void SetUp()
        {
            var container = new Container(x => {
                x.For<SubscriptionSettings>().Use(theSettings);
            });

            var registry = new SubscriptionRegistry();
            registry.StructureMap(container);

            _runtime = registry.ToRuntime();

            theCache = _runtime.Get<ISubscriptionCache>().As<SubscriptionCache>();
        }

        [TearDown]
        public void TearDown()
        {
            _runtime.Dispose();
        }

        [Test]
        public void route_without_any_subscriptions()
        {
            theCache.FindSubscribingChannelsFor(typeof(Message1)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q1);

            theCache.FindSubscribingChannelsFor(typeof(Message2)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q1, theSettings.Q2);

            theCache.FindSubscribingChannelsFor(typeof(Message3)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q2);
        }

        [Test]
        public void route_with_a_single_matching_subscription()
        {
            theCache.LoadSubscriptions(new Subscription[]
            {
                Subscription.For<Message4>().ReceivedBy(theSettings.Q4),
                Subscription.For<Message5>().ReceivedBy(theSettings.Q5)
            });

            theCache.FindSubscribingChannelsFor(typeof(Message4)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q4);


            theCache.FindSubscribingChannelsFor(typeof(Message5)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q5);
        }

        [Test]
        public void with_mixed_subscription_and_static_matching_nodes()
        {
            theCache.LoadSubscriptions(new Subscription[]
            {
                Subscription.For<Message1>().ReceivedBy(theSettings.Q4),
                Subscription.For<Message1>().ReceivedBy(theSettings.Q5)
            });

            theCache.FindSubscribingChannelsFor(typeof(Message1)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q1, theSettings.Q4, theSettings.Q5);
        }

        [Test]
        public void overwrite_subscriptions()
        {
            theCache.LoadSubscriptions(new Subscription[]
            {
                Subscription.For<Message1>().ReceivedBy(theSettings.Q4),
                Subscription.For<Message1>().ReceivedBy(theSettings.Q5)
            });

            theCache.LoadSubscriptions(new Subscription[]
            {
                Subscription.For<Message2>().ReceivedBy(theSettings.Q4),
                Subscription.For<Message1>().ReceivedBy(theSettings.Q5)
            });

            theCache.FindSubscribingChannelsFor(typeof(Message1)).Select(x => x.Uri)
                .ShouldHaveTheSameElementsAs(theSettings.Q1, theSettings.Q5);
        }


        public class SubscriptionRegistry : FubuTransportRegistry<SubscriptionSettings>
        {
            public SubscriptionRegistry()
            {
                ServiceBus.EnableInMemoryTransport();

                Channel(x => x.Q1)
                    .AcceptsMessage<Message1>()
                    .AcceptsMessage<Message2>();

                Channel(x => x.Q2)
                    .AcceptsMessage<Message2>()
                    .AcceptsMessage<Message3>();
            }
        }

        public class SubscriptionSettings
        {
            public Uri Q1 { get; set; }
            public Uri Q2 { get; set; }
            public Uri Q3 { get; set; }
            public Uri Q4 { get; set; }
            public Uri Q5 { get; set; }
        }
    }
}