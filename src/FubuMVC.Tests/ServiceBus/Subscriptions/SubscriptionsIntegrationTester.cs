using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Subscriptions;
using Xunit;
using Shouldly;
using StructureMap;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    
    public class SubscriptionsIntegrationTester : IDisposable
    {
        private FubuRuntime runtime;
        private ISubscriptionCache theRouter;
        private HarnessSettings settings;

        public SubscriptionsIntegrationTester()
        {
            var registry = new RoutedRegistry();

            var container = new Container();
            settings = InMemoryTransport.ToInMemory<HarnessSettings>();
            container.Inject(settings);

            registry.StructureMap(container);

            runtime = registry.ToRuntime();

            theRouter = runtime.Get<ISubscriptionCache>();
        }

        public void Dispose()
        {
            runtime.Dispose();
        }

        [Fact]
        public void if_destination_is_set_on_the_envelope_that_is_the_only_channel_returned()
        {
            var envelope = new Envelope
            {
                Message = new Message1(),
                Destination = settings.Service4
            };

            theRouter.FindDestinationChannels(envelope).Single().Uri.ShouldBe(settings.Service4);
        }

        [Fact]
        public void can_happily_build_and_open_a_new_channel_for_a_destination()
        {
            var envelope = new Envelope
            {
                Message = new Message1(),
                Destination = "memory://dynamic".ToUri()
            };

            theRouter.FindDestinationChannels(envelope).Single().Uri.ShouldBe(envelope.Destination);
        }

        [Fact]
        public void destination_is_specified_but_The_channel_does_not_exist_and_the_transport_is_unknown()
        {
            Exception<UnknownChannelException>.ShouldBeThrownBy(() => {
                var envelope = new Envelope
                {
                    Message = new Message1(),
                    Destination = "unknown://uri".ToUri()
                };

                theRouter.FindDestinationChannels(envelope);
            });
        }

        [Fact]
        public void use_type_rules_on_the_channel_graph_1()
        {
            var envelope = new Envelope {Message = new Message1()};
            theRouter.FindDestinationChannels(envelope).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("Harness:Service1");
        }

        [Fact]
        public void use_type_rules_on_the_channel_graph_2()
        {
            var envelope = new Envelope { Message = new Message2() };
            theRouter.FindDestinationChannels(envelope).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("Harness:Service1", "Harness:Service3");
        }

        [Fact]
        public void use_type_rules_on_the_channel_graph_3()
        {
            var envelope = new Envelope { Message = new Message3() };
            theRouter.FindDestinationChannels(envelope).Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("Harness:Service2", "Harness:Service3");
        }
    }

    public class RoutedRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public RoutedRegistry()
        {
            ServiceBus.EnableInMemoryTransport();

            Channel(x => x.Service1).AcceptsMessage<Message1>();
            Channel(x => x.Service1).AcceptsMessage<Message2>();

            Channel(x => x.Service2).AcceptsMessage<Message3>();
            Channel(x => x.Service2).AcceptsMessage<Message4>();
            
            Channel(x => x.Service3).AcceptsMessage<Message2>();
            Channel(x => x.Service3).AcceptsMessage<Message3>();

            Channel(x => x.Service4).ReadIncoming();

        }
    }
}