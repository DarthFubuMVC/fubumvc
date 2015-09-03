using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class ChannelGraphTester
    {
        [Test]
        public void the_default_content_type_should_be_xml_serialization()
        {
            new ChannelGraph().DefaultContentType.ShouldBe(new XmlMessageSerializer().ContentType);
        }

        [Test]
        public void to_key_by_expression()
        {
            ChannelGraph.ToKey<ChannelSettings>(x => x.Outbound)
                        .ShouldBe("Channel:Outbound");
        }

        [Test]
        public void channel_for_by_accessor()
        {
            var graph = new ChannelGraph();
            var channelNode = graph.ChannelFor<ChannelSettings>(x => x.Outbound);
            channelNode
                 .ShouldBeTheSameAs(graph.ChannelFor<ChannelSettings>(x => x.Outbound));


            channelNode.Key.ShouldBe("Channel:Outbound");
            channelNode.SettingAddress.Name.ShouldBe("Outbound");

        }

        [Test]
        public void reading_settings()
        {
            var channel = new ChannelSettings
            {
                Outbound = new Uri("channel://outbound"),
                Downstream = new Uri("channel://downstream")
            };

            var bus = new BusSettings
            {
                Outbound = new Uri("bus://outbound"),
                Downstream = new Uri("bus://downstream")
            };

            var services = new InMemoryServiceLocator();
            services.Add(channel);
            services.Add(bus);

            var graph = new ChannelGraph();
            graph.ChannelFor<ChannelSettings>(x => x.Outbound);
            graph.ChannelFor<ChannelSettings>(x => x.Downstream);
            graph.ChannelFor<BusSettings>(x => x.Outbound);
            graph.ChannelFor<BusSettings>(x => x.Downstream);

            graph.ReadSettings(services);

            graph.ChannelFor<ChannelSettings>(x => x.Outbound)
                 .Uri.ShouldBe(channel.Outbound);
            graph.ChannelFor<ChannelSettings>(x => x.Downstream)
                 .Uri.ShouldBe(channel.Downstream);
            graph.ChannelFor<BusSettings>(x => x.Outbound)
                .Uri.ShouldBe(bus.Outbound);
            graph.ChannelFor<BusSettings>(x => x.Downstream)
                .Uri.ShouldBe(bus.Downstream);
        }

        [Test, Explicit("Too flaky, too slow. Replace w/ ST tests?")]
        public void start_receiving()
        {
            using (var graph = new ChannelGraph())
            {
                var node1 = graph.ChannelFor<ChannelSettings>(x => x.Upstream);
                var node2 = graph.ChannelFor<ChannelSettings>(x => x.Downstream);
                var node3 = graph.ChannelFor<BusSettings>(x => x.Upstream);
                var node4 = graph.ChannelFor<BusSettings>(x => x.Downstream);

                node1.Incoming = true;
                node2.Incoming = false;
                node3.Incoming = true;
                node4.Incoming = false;

                graph.Each(x => x.Channel = new FakeChannel());

                graph.StartReceiving(MockRepository.GenerateMock<IHandlerPipeline>(), new RecordingLogger());

                node1.Channel.As<FakeChannel>().ReceivedCount.ShouldBeGreaterThan(0);
                node2.Channel.As<FakeChannel>().ReceivedCount.ShouldBe(0);
                node3.Channel.As<FakeChannel>().ReceivedCount.ShouldBeGreaterThan(0);
                node4.Channel.As<FakeChannel>().ReceivedCount.ShouldBe(0);
            }
        }

        public class FakeChannel : IChannel
        {
            public void Dispose()
            {
            }

            public Uri Address { get; private set; }
            public ReceivingState Receive(IReceiver receiver)
            {
                ReceivedCount++;
                return ReceivingState.CanContinueReceiving;
            }

            public int ReceivedCount { get; set; }

            public void Send(byte[] data, IHeaders headers)
            {
            }
        }


    }

    public class ChannelSettings
    {
        public Uri Outbound { get; set; }
        public Uri Downstream { get; set; }
        public Uri Upstream { get; set; }

        public int UpstreamCount { get; set; }
        public int OutboundCount { get; set; }
    }

    public class BusSettings
    {
        public Uri Outbound { get; set; }
        public Uri Downstream { get; set; }
        public Uri Upstream { get; set; }
    }
}