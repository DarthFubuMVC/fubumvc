using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class FubuTransportRegistry_register_subscription_requirements_Tester
    {
        private FubuRuntime runtime;
        private Container container;
        private readonly BusSettings theSettings = InMemoryTransport.ToInMemory<BusSettings>();

        [TestFixtureSetUp]
        public void SetUp()
        {
            container = new Container(x => {
                x.For<BusSettings>().Use(theSettings);
            });

            var registry = new SubscribedRegistry();
            registry.StructureMap(container);
            runtime = registry.ToRuntime();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            runtime.Dispose();
        }


        [Test]
        public void the_expected_subscriptions()
        {
            var graph = container.GetInstance<ChannelGraph>();

            var actual = container.GetAllInstances<ISubscriptionRequirement>()
                .SelectMany(x => x.DetermineRequirements()).ToArray();

            var expected = new Subscription[]
            {
                new Subscription(typeof(Message1)){NodeName = "SubscribedService", Receiver = InMemoryTransport.ReplyUriForGraph(graph), Source = theSettings.Outbound},
                new Subscription(typeof(Message3)){NodeName = "SubscribedService", Receiver = InMemoryTransport.ReplyUriForGraph(graph), Source = theSettings.Outbound},
                new Subscription(typeof(Message2)){NodeName = "SubscribedService", Receiver = theSettings.Inbound, Source = theSettings.Upstream},
                new Subscription(typeof(Message4)){NodeName = "SubscribedService", Receiver = theSettings.Inbound, Source = theSettings.Upstream},
            };

            actual.ShouldHaveTheSameElementsAs(expected);
        }



        public class SubscribedRegistry : FubuTransportRegistry<BusSettings>
        {
            public SubscribedRegistry()
            {
                ServiceBus.EnableInMemoryTransport();
                NodeName = "SubscribedService";

                Channel(x => x.Inbound).ReadIncoming();

                SubscribeLocally()
                    .ToSource(x => x.Outbound)
                    .ToMessage<Message1>()
                    .ToMessage<Message3>();

                SubscribeAt(x => x.Inbound)
                    .ToSource(x => x.Upstream)
                    .ToMessage<Message2>()
                    .ToMessage<Message4>();
            }
        }

        public class BusSettings
        {
            public Uri Inbound { get; set; }
            public Uri Outbound { get; set; }
            public Uri Upstream { get; set; }
        }
    }
}