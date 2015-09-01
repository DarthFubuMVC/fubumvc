using System;
using System.Collections.Generic;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime
{
    [TestFixture]
    public class ReceiverContentTypeHandling
    {
        private ChannelGraph theGraph;
        private ChannelNode theNode;
        private RecordingHandlerPipeline theInvoker;
        private Receiver theReceiver;
        private IMessageCallback theCallback;
        private RecordingLogger theLogger;

        [SetUp]
        public void SetUp()
        {
            theGraph = new ChannelGraph();
            theNode = new ChannelNode();
            theNode.Channel = new InMemoryChannel(new Uri("memory://foo"));

            theInvoker = new RecordingHandlerPipeline();

            theCallback = MockRepository.GenerateMock<IMessageCallback>();
            theLogger = new RecordingLogger();

            theReceiver = new Receiver(theInvoker, theGraph, theNode);
        }

        [Test]
        public void if_no_content_type_is_specified_on_envelope_or_channel_use_graph_default()
        {
            theGraph.DefaultContentType = "text/json";
            theNode.DefaultContentType = null;

            var headers = new NameValueHeaders();
            theReceiver.Receive(new byte[0], headers, MockRepository.GenerateMock<IMessageCallback>());

            headers[Envelope.ContentTypeKey].ShouldBe("text/json");
        }

        [Test]
        public void if_no_content_type_is_specified_use_channel_default_when_it_exists()
        {
            theGraph.DefaultContentType = "text/json";
            theNode.DefaultContentType = "text/xml";


            var headers = new NameValueHeaders();
            theReceiver.Receive(new byte[0], headers, MockRepository.GenerateMock<IMessageCallback>());

            headers[Envelope.ContentTypeKey].ShouldBe("text/xml");
        }

        [Test]
        public void the_envelope_content_type_wins()
        {
            theGraph.DefaultContentType = "text/json";
            theNode.DefaultContentType = "text/xml";


            var headers = new NameValueHeaders();
            headers[Envelope.ContentTypeKey] = "text/plain";
            theReceiver.Receive(new byte[0], headers, MockRepository.GenerateMock<IMessageCallback>());

            headers[Envelope.ContentTypeKey].ShouldBe("text/plain");
        }

    }

    public class RecordingHandlerPipeline : IHandlerPipeline, IInvocationContext
    {
        public IList<Envelope> Invoked = new List<Envelope>();

        public IList<object> Responses = new List<object>(); 


        public void Invoke(Envelope envelope, IEnvelopeContext context)
        {
            Invoked.Add(envelope);
        }

        public void Receive(Envelope envelope)
        {
            Invoke(envelope, null);
        }

        public void InvokeNow(Envelope envelope)
        {
            throw new NotImplementedException();
        }

        public void InvokeNow<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void ExecuteChain(Envelope envelope, HandlerChain chain)
        {
            throw new NotImplementedException();
        }

        public HandlerChain FindChain(Envelope envelope)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return Responses.GetEnumerator();
        }

        public void EnqueueCascading(object message)
        {
            Responses.Add(message);
        }

        public IEnumerable<object> OutgoingMessages()
        {
            return Responses;
        }

        // just to satisfy the interface
        public Envelope Envelope { get; set; }
        public IContinuation Continuation { get; set; }

        public bool Matches(Envelope envelope)
        {
            throw new NotImplementedException();
        }

        public IContinuation Handle(Envelope envelope)
        {
            throw new NotImplementedException();
        }
    }


    [TestFixture]
    public class when_receiving_a_message : InteractionContext<Receiver>
    {
        Envelope envelope = new Envelope();
        Uri address = new Uri("foo://bar");
        private IChannel theChannel;
        private ChannelNode theNode;
        private IMessageCallback theCallback;
        private byte[] theData;
        private NameValueHeaders theHeaders;

        protected override void beforeEach()
        {
            theChannel = MockFor<IChannel>();
            theChannel.Stub(x => x.Address).Return(address);

            theNode = new ChannelNode
            {
                DefaultContentType = "application/json",
                Channel = theChannel,
                Uri = address
            };

            Services.Inject(theNode);

            MockFor<IChainInvoker>().Stub(x => x.Invoke(null)).IgnoreArguments();

            theCallback = MockRepository.GenerateMock<IMessageCallback>();
            theData = new byte[] {1, 2, 3};
            theHeaders = new NameValueHeaders();
            theHeaders[Envelope.AttemptsKey] = "2";

            ClassUnderTest.Receive(theData, theHeaders, theCallback);
        }

        [Test]
        public void should_copy_the_channel_address_to_the_envelope()
        {
            new HeaderWrapper{Headers = theHeaders}.ReceivedAt.ShouldBe(address);
        }

        [Test]
        public void should_call_through_to_the_pipeline()
        {
            MockFor<IHandlerPipeline>().AssertWasCalled(x => x.Receive(new Envelope(theData, theHeaders, theCallback)));
        }

    }
}