using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Sagas;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration;
using FubuMVC.Core.ServiceBus.Web;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus
{
    public enum InMemoryTransportMode
    {
        Disabled,
        Enabled,
        AllInMemory
    }

    public class TransportSettings : IFeatureSettings
    {
        public readonly IList<ISagaStorage> SagaStorageProviders;
        public readonly IList<Type> SettingTypes = new List<Type>();

        public TransportSettings()
        {
            SagaStorageProviders = new List<ISagaStorage>();
            DebugEnabled = false;
            DelayMessagePolling = 5000;
            ListenerCleanupPolling = 60000;
            SubscriptionRefreshPolling = 60000;
            Enabled = false;
            InMemoryTransport = InMemoryTransportMode.Disabled;
        }

        
        public bool Enabled { get; set; }

        public InMemoryTransportMode InMemoryTransport { get; set; }

        public Uri InMemoryReplyUri { get; set; }

        public bool DebugEnabled { get; set; }
        public double DelayMessagePolling { get; set; }
        public double ListenerCleanupPolling { get; set; }
        public double SubscriptionRefreshPolling { get; set; }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (!Enabled) return;

            registry.Actions.FindWith<SendsMessageActionSource>();
            registry.Policies.Global.Add<SendsMessageConvention>();

            registry.Policies.Global.Add<ApplyScheduledJobRouting>();
            registry.Services.IncludeRegistry<ScheduledJobServicesRegistry>();
            registry.Services.IncludeRegistry<MonitoringServiceRegistry>();
            registry.Policies.ChainSource<SystemLevelHandlers>();
            registry.Services.IncludeRegistry(new FubuTransportServiceRegistry(registry.Mode));
            registry.Services.IncludeRegistry<PollingServicesRegistry>();
            registry.Policies.Global.Add<StatefulSagaConvention>();
            registry.Policies.Global.Add<AsyncHandlingConvention>();

            // Just forcing it to get spun up.
            registry.AlterSettings<ChannelGraph>(x => {});
            registry.Handlers.Include<SystemLevelHandlers>();

            registry.Configure(graph =>
            {
                graph.Handlers.Each(chain =>
                {
                    // Apply the error handling node
                    chain.InsertFirst(new ExceptionHandlerNode(chain));

                    // Hate how we're doing this, but disable tracing
                    // on the polling job requests here.
                    if (chain.InputType().Closes(typeof (JobRequest<>)))
                    {
                        chain.Tags.Add(BehaviorChain.NoTracing);
                    }
                });
            });

            if (InMemoryTransport == InMemoryTransportMode.AllInMemory)
            {
                registry.Services.For<ITransport>().ClearAll();
                registry.Services.AddService<ITransport, InMemoryTransport>();

                SettingTypes.Each(settingType =>
                {
                    var settings = InMemory.InMemoryTransport.ToInMemory(settingType);
                    registry.Services.ReplaceService(settingType, new ObjectInstance(settings));
                });
            }

            if (InMemoryTransport != InMemoryTransportMode.Disabled)
            {
                registry.Services.AddService<ITransport, InMemoryTransport>();
            }

            registry.Policies.Global.Add<ReorderBehaviorsPolicy>(x =>
            {
                x.ThisNodeMustBeBefore<StatefulSagaNode>();
                x.ThisNodeMustBeAfter<HandlerCall>();
            });
        }

        

        public interface SetInMemory
        {
            void SetToInMemory(FubuRegistry registry);
        }

        public class SetInMemory<T> : SetInMemory where T : class, new()
        {
            public void SetToInMemory(FubuRegistry registry)
            {
                registry.AlterSettings<T>(x => InMemory.InMemoryTransport.AllChannelsAreInMemory(typeof(T), x));
            }
        }
    }

    
}