using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    [TestFixture]
    public class when_creating_grouped_subscriptions
    {
        public BusSettings theSettings = InMemoryTransport.ToInMemory<BusSettings>();
        private ChannelGraph theGraph;
        private IEnumerable<Subscription> theSubscriptions;

        [SetUp]
        public void SetUp()
        {
            theGraph = new ChannelGraph { Name = "FooNode" };

            var requirement = new GroupSubscriptionRequirement<BusSettings>(x => x.Upstream, x => x.Incoming);
            requirement.AddType(typeof(FooMessage));
            requirement.AddType(typeof(BarMessage));

            theSubscriptions = requirement.Determine(theSettings, theGraph);
        }

        [Test]
        public void should_set_the_receiver_uri_to_the_explicitly_chosen_uri()
        {
            theSubscriptions.First().Receiver
                .ShouldBe(theSettings.Incoming);
        }

        [Test]
        public void sets_the_node_name_from_the_channel_graph()
        {
            theSubscriptions.Select(x => x.NodeName).Distinct()
                .Single().ShouldBe(theGraph.Name);
        }

        [Test]
        public void should_set_the_source_uri_to_the_requested_source_from_settings()
        {
            theSubscriptions.First().Source
                .ShouldBe(theSettings.Upstream);

        }

        [Test]
        public void should_add_a_subscription_for_each_type()
        {
            theSubscriptions.Select(x => x.MessageType)
                .ShouldHaveTheSameElementsAs(typeof(FooMessage).GetFullName(), typeof(BarMessage).GetFullName());
        }

        public class BusSettings
        {
            public Uri Outbound { get; set; }
            public Uri Incoming { get; set; }
            public Uri Upstream { get; set; }
        }

        public class FooMessage { }
        public class BarMessage { }
    }
}