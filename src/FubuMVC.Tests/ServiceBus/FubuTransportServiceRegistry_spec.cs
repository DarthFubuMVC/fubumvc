using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Core.ServiceBus.TestSupport;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FubuTransportServiceRegistry_spec
    {
        [Test]
        public void service_registrations()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                var c = runtime.Get<IContainer>();



                c.DefaultSingletonIs<ISubscriptionCache, SubscriptionCache>();
                c.DefaultSingletonIs<ISagaStateCacheFactory, SagaStateCacheFactory>();
                c.DefaultSingletonIs<IEventAggregator, EventAggregator>();

                c.DefaultRegistrationIs<IServiceBus, Core.ServiceBus.ServiceBus>();
                c.DefaultRegistrationIs<IEnvelopeSender, EnvelopeSender>();
                c.DefaultRegistrationIs<IEnvelopeSerializer, EnvelopeSerializer>();
                c.DefaultRegistrationIs<IChainInvoker, ChainInvoker>();
                c.DefaultRegistrationIs<IMessageSerializer, XmlMessageSerializer>();
                c.DefaultRegistrationIs<IHandlerPipeline, HandlerPipeline>();
                c.DefaultRegistrationIs<IMessageExecutor, MessageExecutor>();
                c.DefaultRegistrationIs<IOutgoingSender, OutgoingSender>();
                c.DefaultRegistrationIs<IAsyncHandling, AsyncHandling>();
                c.DefaultRegistrationIs<ISubscriptionRepository, SubscriptionRepository>();



                c.ShouldHaveRegistration<IActivator, ServiceBusActivator>();
                c.ShouldHaveRegistration<ILogListener, EventAggregationListener>();

                c.ShouldNotHaveRegistration<IActivator, TransportCleanupActivator>();

            }
        }

        


        [Test]
        public void use_synchronous_event_aggregator_if_in_testing_mode()
        {
            using (var runtime = FubuRuntime.BasicBus(x => x.Mode = "testing"))
            {
                var c = runtime.Get<IContainer>();

                c.DefaultSingletonIs<IEventAggregator, SynchronousEventAggregator>();

            }
        }


        [Test]
        public void service_registrations_when_mode_is_testing()
        {

            using (var runtime = FubuRuntime.BasicBus(x => x.Mode = "testing"))
            {
                var c = runtime.Get<IContainer>();

                c.ShouldHaveRegistration<IListener, MessageWatcher>();
                c.DefaultRegistrationIs<IMessagingSession, MessagingSession>();
                c.ShouldHaveRegistration<ILogListener, MessageRecordListener>();
            }
        }



        [Test]
        public void service_registrations_when_not_in_testing_mode()
        {

            using (var runtime = FubuRuntime.BasicBus())
            {
                var c = runtime.Get<IContainer>();

                c.ShouldNotHaveRegistration<IListener, MessageWatcher>();
                c.Model.HasImplementationsFor<IMessagingSession>().ShouldBeFalse();
                c.ShouldNotHaveRegistration<ILogListener, MessageRecordListener>();
            }
        }

        [Test]
        public void synchronous_event_aggregator_is_used_in_testing_mode()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {

                _.Mode = "testing";
                _.ServiceBus.Configure(x =>
                {
                    x.Enabled = true;
                    x.InMemoryTransport = InMemoryTransportMode.Enabled;
                });
            }))
            {
                runtime.Get<IContainer>().DefaultSingletonIs<IEventAggregator, SynchronousEventAggregator>();
            }
        }

        [Test]
        public void TransportCleanupActivator_is_registered_if_FubuTransport_testing_mode_is_on()
        {
            using (var runtime = FubuRuntime.Basic(_ =>
            {

                _.Mode = "testing";
                _.ServiceBus.Configure(x =>
                {
                    x.Enabled = true;
                    x.InMemoryTransport = InMemoryTransportMode.Enabled;
                });
            }))
            {
                runtime.Get<IContainer>().ShouldHaveRegistration<IActivator, TransportCleanupActivator>();
            }
        }


    }
}