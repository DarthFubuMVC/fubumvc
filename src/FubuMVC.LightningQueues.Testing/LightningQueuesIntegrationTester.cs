using System;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.LightningQueues.Queues;
using FubuMVC.Tests.ServiceBus;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    
    public class LightningQueuesIntegrationTester : IDisposable
    {
        private void SetupTransport(string uri, ChannelMode mode)
        {
            graph = new ChannelGraph();
            node = graph.ChannelFor<ChannelSettings>(x => x.Upstream);
            node.Mode = mode;

            node.Uri = new Uri(uri);
            node.Incoming = true;

            var delayedCache = new DelayedMessageCache<MessageId>();
            queues = new PersistentQueues(new RecordingLogger());
            queues.ClearAll();
            transport = new LightningQueuesTransport(queues, new LightningQueueSettings());

            transport.OpenChannels(graph);
        }

        public void Dispose()
        {
            queues.Dispose();
        }

        private PersistentQueues queues;
        private LightningQueuesTransport transport;
        private ChannelGraph graph;
        private ChannelNode node;

        [Fact]
        public void registers_a_reply_queue_corrected_to_the_machine_name()
        {
            SetupTransport("lq.tcp://localhost:2032/upstream", ChannelMode.DeliveryGuaranteed);

            var uri = graph.ReplyChannelFor(LightningUri.Protocol);
            uri.ShouldNotBeNull();

            uri.Host.ToUpperInvariant().ShouldBe(Environment.MachineName.ToUpperInvariant());
        }

        [Fact]
        public void reply_uri_is_machine_specific_when_dns_address_is_used()
        {
            SetupTransport("lq.tcp://www.foo.com:2032/upstream", ChannelMode.DeliveryGuaranteed);

            var uri = graph.ReplyChannelFor(LightningUri.Protocol);
            uri.Host.ToUpperInvariant().ShouldBe(Environment.MachineName.ToUpperInvariant());
        }

        [Fact]
        public void send_a_message_and_get_it_back()
        {
            SetupTransport("lq.tcp://localhost:2032/upstream", ChannelMode.DeliveryGuaranteed);

            var envelope = new Envelope { Data = new byte[] { 1, 2, 3, 4, 5 } };
            envelope.Headers["foo"] = "bar";

            var receiver = new RecordingReceiver();

            node.StartReceiving(receiver, new RecordingLogger());

            node.Channel.As<LightningQueuesChannel>().Send(envelope.Data, envelope.Headers);
            Wait.Until(() => receiver.Received.Any());

            graph.Dispose();
            queues.Dispose();

            receiver.Received.Any().ShouldBeTrue();

            var actual = receiver.Received.Single();
            actual.Data.ShouldBe(envelope.Data);
            actual.Headers["foo"].ShouldBe("bar");
        }


        [Fact]
        public void send_a_message_and_get_it_back_non_persistent()
        {
            SetupTransport("lq.tcp://localhost:2032/upstream", ChannelMode.DeliveryFastWithoutGuarantee);

            var envelope = new Envelope { Data = new byte[] { 1, 2, 3, 4, 5 } };
            envelope.Headers["foo"] = "bar";

            var receiver = new RecordingReceiver();

            node.StartReceiving(receiver, new RecordingLogger());

            node.Channel.As<LightningQueuesChannel>().Send(envelope.Data, envelope.Headers);
            Wait.Until(() => receiver.Received.Any());

            graph.Dispose();
            queues.Dispose();

            receiver.Received.Any().ShouldBeTrue();

            var actual = receiver.Received.Single();
            actual.Data.ShouldBe(envelope.Data);
            actual.Headers["foo"].ShouldBe("bar");
        }
    }

    public class ChannelSettings
    {
        public Uri Outbound { get; set; }
        public Uri Downstream { get; set; }
        public Uri Upstream { get; set; }
    }
}