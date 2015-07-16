using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTransportation.Async;
using FubuTransportation.Configuration;
using FubuTransportation.Diagnostics;
using FubuTransportation.Events;
using FubuTransportation.InMemory;
using FubuTransportation.Logging;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Cascading;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Runtime.Serializers;
using FubuTransportation.Subscriptions;
using FubuTransportation.TestSupport;

namespace FubuTransportation
{
    public class FubuTransportServiceRegistry : ServiceRegistry
    {
        public FubuTransportServiceRegistry()
        {
            var eventAggregatorDef = FubuTransport.UseSynchronousLogging 
                ? ObjectDef.ForType<SynchronousEventAggregator>() 
                : ObjectDef.ForType<EventAggregator>();
            
            eventAggregatorDef.IsSingleton = true;
            SetServiceIfNone(typeof(IEventAggregator), eventAggregatorDef);

            

            var stateCacheDef = new ObjectDef(typeof(SagaStateCacheFactory));
            stateCacheDef.IsSingleton = true;
            SetServiceIfNone(typeof(ISagaStateCacheFactory), stateCacheDef);

            SetServiceIfNone<IChainInvoker, ChainInvoker>();
            SetServiceIfNone<IEnvelopeSender, EnvelopeSender>();
            AddService<IMessageSerializer, XmlMessageSerializer>();

            AddService<IActivator, FubuTransportationActivator>();

            SetServiceIfNone<IServiceBus, ServiceBus>();

            SetServiceIfNone<IEnvelopeSerializer, EnvelopeSerializer>();
            SetServiceIfNone<IHandlerPipeline, HandlerPipeline>();


            AddService<ILogListener, EventAggregationListener>();

            if (FubuTransport.ApplyMessageHistoryWatching || FubuMode.InTestingMode())
            {
                AddService<IListener, MessageWatcher>();

                var def = ObjectDef.ForType<MessagingSession>();
                def.IsSingleton = true;
                SetServiceIfNone(typeof(IMessagingSession), def);
                AddService<ILogListener, MessageRecordListener>();
            }


            if (FubuMode.InTestingMode())
            {
                AddService<IActivator, TransportCleanupActivator>();
            }

            AddService<IEnvelopeHandler, DelayedEnvelopeHandler>();
            AddService<IEnvelopeHandler, ResponseEnvelopeHandler>();
            AddService<IEnvelopeHandler, ChainExecutionEnvelopeHandler>();
            AddService<IEnvelopeHandler, NoSubscriberHandler>();

            SetServiceIfNone<IMessageExecutor, MessageExecutor>();
            SetServiceIfNone<IOutgoingSender, OutgoingSender>();

            SetServiceIfNone<IAsyncHandling, AsyncHandling>();

            subscriptions();
        }

        private void subscriptions()
        {
            var subscriberDef = ObjectDef.ForType<SubscriptionCache>();
            subscriberDef.IsSingleton = true;
            SetServiceIfNone(typeof (ISubscriptionCache), subscriberDef);

            SetServiceIfNone<ISubscriptionRepository, SubscriptionRepository>();

            SetServiceIfNone<ISubscriptionPersistence>(new InMemorySubscriptionPersistence());

        }
    }
}