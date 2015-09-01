using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class when_activating_the_transport_subsystem : InteractionContext<TransportActivator>
    {
        private ChannelGraph theGraph;

        protected override void beforeEach()
        {
            theGraph = MockRepository.GenerateMock<ChannelGraph>();

            Services.Inject(theGraph);
            Services.PartialMockTheClassUnderTest();

             ClassUnderTest.Stub(x => x.OpenChannels());
            ClassUnderTest.Stub(x => x.ExecuteActivators());

            ClassUnderTest.Activate(new ActivationLog(), null);
        }

        [Test]
        public void reads_the_settings()
        {
            theGraph.AssertWasCalled(x => x.ReadSettings(MockFor<IServiceLocator>()));
        }

        [Test]
        public void should_start_the_channels()
        {
            ClassUnderTest.AssertWasCalled(x => x.OpenChannels());
        }

        [Test]
        public void should_start_receiving()
        {
            theGraph.AssertWasCalled(x => x.StartReceiving(MockFor<IHandlerPipeline>(), MockFor<ILogger>()));
        }

        [Test]
        public void should_invoke_activators()
        {
            ClassUnderTest.AssertWasCalled(x => x.ExecuteActivators());
        }
    }

    [TestFixture]
    public class when_starting_the_subscriptions : InteractionContext<TransportActivator>
    {
        private ChannelGraph theGraph;
        private ITransport[] theTransports;

        protected override void beforeEach()
        {
            theGraph = new ChannelGraph();
            Services.Inject(theGraph);
            theTransports = Services.CreateMockArrayFor<ITransport>(5);

            ClassUnderTest.OpenChannels();
        }


        [Test]
        public void starts_each_transport()
        {
            theTransports.Each(transport => transport.AssertWasCalled(x => x.OpenChannels(theGraph)));
        }
    }

    [TestFixture]
    public class when_starting_the_subscriptions_and_there_are_unknown_channels
    {
        [Test]
        public void should_throw_an_exception_listing_the_channels_that_are_missing()
        {
            var graph = new ChannelGraph();
            graph.Add(new ChannelNode
            {
                Key = "Foo:1",
                Uri = "foo://1".ToUri()
            });

            graph.Add(new ChannelNode
            {
                Key = "Foo:2",
                Uri = "foo://2".ToUri()
            });

            var subscriptions = new TransportActivator(graph, null, null, new RecordingLogger(), 
                new ITransport[] {new InMemoryTransport()},
                Enumerable.Empty<IFubuTransportActivator>());


            Exception<InvalidOrMissingTransportException>.ShouldBeThrownBy(subscriptions.OpenChannels);
        }
    }

    [TestFixture]
    public class when_starting_the_subscriptions_and_there_are_activators :
        InteractionContext<TransportActivator>
    {
        private IFubuTransportActivator[] theActivators;

        protected override void beforeEach()
        {
            theActivators = Services.CreateMockArrayFor<IFubuTransportActivator>(5);
            ClassUnderTest.ExecuteActivators();
        }

        [Test]
        public void invokes_each_activator()
        {
            theActivators.Each(activator => activator.AssertWasCalled(x => x.Activate()));
        }
    }
}