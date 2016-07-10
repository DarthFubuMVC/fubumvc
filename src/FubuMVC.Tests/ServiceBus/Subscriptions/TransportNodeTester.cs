using System;
using System.Linq;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    [TestFixture]
    public class TransportNodeTester
    {
        [Test]
        public void use_the_explicit_control_queue_if_it_exists()
        {
            var graph = new ChannelGraph
            {
                NodeId = "Foo@Bar"

            };

            graph.AddReplyChannel(InMemoryChannel.Protocol, "memory://localhost/replies".ToUri());

            graph.ControlChannel = new ChannelNode {Uri = "memory://1".ToUri()};

            var node = new TransportNode(graph);

            node.ControlChannel.ShouldBe("memory://1".ToUri());
        }

        [Test]
        public void fallback_to_the_first_incoming_channel_for_control_queue_if_no_explicit_control_queue_exists()
        {
            var graph = new ChannelGraph
            {
                NodeId = "Foo@Bar"

            };

            graph.AddReplyChannel(InMemoryChannel.Protocol, "memory://localhost/replies".ToUri());

            //graph.ControlChannel = new ChannelNode { Uri = "memory://1".ToUri() };

            var node = new TransportNode(graph);

            node.ControlChannel.ShouldBe("memory://localhost/replies".ToUri());
        }

        [Test]
        public void build_with_channel_graph_sets_the_id_to_the_node_id()
        {
            var graph = new ChannelGraph
            {
                NodeId = "Foo@Bar"
                
            };

            graph.AddReplyChannel(InMemoryChannel.Protocol, "memory://localhost/replies".ToUri());

            var node = new TransportNode(graph);

            node.Id.ShouldBe(graph.NodeId);
        }

        [Test]
        public void blow_up_if_no_reply_channels()
        {
            var graph = new ChannelGraph
            {
                NodeId = "Foo@Bar"

            };

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => {
                new TransportNode(graph);
            }).Message.ShouldBe("At least one reply channel is required");
        }

        [Test]
        public void create_a_transport_node_from_a_channel_graph()
        {
            var graph = new ChannelGraph
            {
                Name = "Service1"
            };

            graph.AddReplyChannel("memory", "memory://replies".ToUri());
            graph.AddReplyChannel("foo", "foo://replies".ToUri());
            graph.AddReplyChannel("bar", "bar://replies".ToUri());

            var node = new TransportNode(graph);

            node.NodeName.ShouldBe("Service1");

            node.Addresses.OrderBy(x => x.ToString()).ShouldHaveTheSameElementsAs("bar://replies".ToUri(), "foo://replies".ToUri(), "memory://replies".ToUri());
        }

        
    }
}