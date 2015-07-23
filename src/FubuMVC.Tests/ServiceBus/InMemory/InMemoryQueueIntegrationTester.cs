using System;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using System.Linq;
using Shouldly;

namespace FubuTransportation.Testing.InMemory
{
    [TestFixture]
    public class InMemoryQueueIntegrationTester
    {
        [SetUp]
        public void SetUp()
        {
            InMemoryQueueManager.ClearAll();
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryQueueManager.ClearAll();
        }

        [Test]
        public void can_round_trip_an_envelope_through_the_queue()
        {
            var envelope = new EnvelopeToken();
            envelope.CorrelationId = Guid.NewGuid().ToString();
            envelope.Headers["Foo"] = "Bar";
            envelope.Data = new byte[] { 1, 2, 3, 4, 5 };

            var queue = new InMemoryQueue(new Uri("memory://foo"));

            var receiver = new RecordingReceiver();
            var task = Task.Factory.StartNew(() => queue.Receive(receiver));

            queue.Enqueue(envelope);

            Wait.Until(() => receiver.Received.Count > 0, timeoutInMilliseconds: 2000);

            var received = receiver.Received.Single();

            received.CorrelationId.ShouldBe(envelope.CorrelationId);
            received.ContentType.ShouldBe(envelope.ContentType);
            received.Data.ShouldBe(envelope.Data);
            task.SafeDispose();
        }

        [Test]
        public void create_from_graph_and_run_through_the_channel()
        {
            using (var graph = new ChannelGraph())
            {
                var node = graph.ChannelFor<BusSettings>(x => x.Outbound);

                node.Uri = new Uri("memory://foo");

                var transport = new InMemoryTransport();
                transport.OpenChannels(graph);
                node.Channel.ShouldNotBeNull();

                var envelope = new Envelope();
                envelope.CorrelationId = Guid.NewGuid().ToString();
                envelope.Headers["Foo"] = "Bar";
                envelope.Data = new byte[] { 1, 2, 3, 4, 5 };

                var receiver = new RecordingReceiver();

                node.StartReceiving(receiver, new RecordingLogger());

                node.Channel.Send(envelope.Data, envelope.Headers);

                Wait.Until(() => receiver.Received.Any(), timeoutInMilliseconds: 2000);

                var received = receiver.Received.Single();

                received.CorrelationId.ShouldBe(envelope.CorrelationId);
                received.ContentType.ShouldBe(envelope.ContentType);
                received.Data.ShouldBe(envelope.Data);

            }
        }

        [Test]
        public void can_be_used_after_clearing_all_messages()
        {
            using (var graph = new ChannelGraph())
            {
                var node = graph.ChannelFor<BusSettings>(x => x.Outbound);
                node.Uri = new Uri("memory://foo");

                var transport = new InMemoryTransport();
                transport.OpenChannels(graph);

                var receiver = new RecordingReceiver();
                node.StartReceiving(receiver, new RecordingLogger());

                node.Channel.Send(new byte[] { 1, 2 }, new NameValueHeaders());

                transport.ClearAll();

                node.Channel.Send(new byte[] { 3, 4 }, new NameValueHeaders());

                Wait.Until(() => receiver.Received.Count == 2);
                receiver.Received.ShouldHaveCount(2);
            }
        }

        [Test]
        public void can_reuse_queues()
        {
            var uri = new Uri("memory://foo");
            var queue = InMemoryQueueManager.QueueFor(uri);
            queue.Enqueue(new EnvelopeToken());
            queue.Clear();
            queue.Dispose();

            queue = InMemoryQueueManager.QueueFor(uri);
            queue.Enqueue(new EnvelopeToken());
            queue.Peek().ShouldHaveCount(1);
        }
    }


}