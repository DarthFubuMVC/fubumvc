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

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FubuTransportServiceRegistry_spec
    {
        [Test]
        public void service_registrations()
        {
            FubuTransport.Reset();

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                var c = runtime.Container;



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
        public void use_synchronous_event_aggregator_if_FubuTransport_UseSynchronousLogging()
        {
            FubuTransport.UseSynchronousLogging = true;

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                var c = runtime.Container;

                c.DefaultSingletonIs<IEventAggregator, SynchronousEventAggregator>();

            }
        }


        [Test]
        public void service_registrations_when_FubuTransport_messagewatching_is_applied()
        {
            FubuTransport.ApplyMessageHistoryWatching = true;

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                var c = runtime.Container;

                c.ShouldHaveRegistration<IListener, MessageWatcher>();
                c.DefaultRegistrationIs<IMessagingSession, MessagingSession>();
                c.ShouldHaveRegistration<ILogListener, MessageRecordListener>();
            }
        }



        [Test]
        public void service_registrations_when_FubuTransport_messagewatching_is_NOT_applied()
        {
            FubuTransport.ApplyMessageHistoryWatching = false;

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                var c = runtime.Container;

                c.ShouldNotHaveRegistration<IListener, MessageWatcher>();
                c.Model.HasImplementationsFor<IMessagingSession>().ShouldBeFalse();
                c.ShouldNotHaveRegistration<ILogListener, MessageRecordListener>();
            }
        }

        [Test]
        public void TransportCleanupActivator_is_registered_if_FubuTransport_testing_mode_is_on()
        {
            FubuMode.SetupForTestingMode();

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                runtime.Container.ShouldHaveRegistration<IActivator, TransportCleanupActivator>();
            }
        }


    }
}