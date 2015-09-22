using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Registration;
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
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus
{
    public class FubuTransportServiceRegistry : ServiceRegistry
    {
        public FubuTransportServiceRegistry(string mode)
        {
            var eventAggregatorDef =mode.InTesting()
                ? new SmartInstance<SynchronousEventAggregator>()
                : (Instance)new SmartInstance<EventAggregator>();
            
            eventAggregatorDef.SetLifecycleTo<SingletonLifecycle>();
            SetServiceIfNone(typeof(IEventAggregator), eventAggregatorDef);



            var stateCacheDef = new SmartInstance<SagaStateCacheFactory>();
            stateCacheDef.Singleton();
            SetServiceIfNone(typeof(ISagaStateCacheFactory), stateCacheDef);

            SetServiceIfNone<IChainInvoker, ChainInvoker>();
            SetServiceIfNone<IEnvelopeSender, EnvelopeSender>();
            AddService<IMessageSerializer, XmlMessageSerializer>();

            AddService<IActivator, ServiceBusActivator>();

            SetServiceIfNone<IServiceBus, ServiceBus>();

            SetServiceIfNone<IEnvelopeSerializer, EnvelopeSerializer>();
            SetServiceIfNone<IHandlerPipeline, HandlerPipeline>();


            AddService<ILogListener, EventAggregationListener>();

            if (mode.InTesting())
            {
                AddService<IListener, MessageWatcher>();

                var def = new SmartInstance<MessagingSession>();
                def.Singleton();
                SetServiceIfNone(typeof(IMessagingSession), def);
                AddService<ILogListener, MessageRecordListener>();
            }


            if (mode.InTesting())
            {
                AddService<IActivator, TransportCleanupActivator>();
            }

            AddService<IEnvelopeHandler, DelayedEnvelopeHandler>();
            AddService<IEnvelopeHandler, ResponseEnvelopeHandler>();
            AddService<IEnvelopeHandler, ChainExecutionEnvelopeHandler>();
            AddService<IEnvelopeHandler, NoSubscriberHandler>();

            SetServiceIfNone<IMessageExecutor, MessageExecutor>();
            SetServiceIfNone<IOutgoingSender, OutgoingSender>();

            

            subscriptions();
        }

        private void subscriptions()
        {
            var subscriberDef = new SmartInstance<SubscriptionCache>();
            subscriberDef.Singleton();
            SetServiceIfNone(typeof (ISubscriptionCache), subscriberDef);

            SetServiceIfNone<ISubscriptionRepository, SubscriptionRepository>();

            SetServiceIfNone<ISubscriptionPersistence>(new InMemorySubscriptionPersistence());

        }
    }
}