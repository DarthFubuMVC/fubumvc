using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
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
    [ApplicationLevel]
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
        }

        public bool Enabled { get; set; }

        public bool EnableInMemoryTransport { get; set; }

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
            registry.Services<ScheduledJobServicesRegistry>();
            registry.Services<MonitoringServiceRegistry>();
            registry.Policies.ChainSource<ImportHandlers>();
            registry.Services<FubuTransportServiceRegistry>();
            registry.Services<PollingServicesRegistry>();
            registry.Policies.Global.Add<StatefulSagaConvention>();
            registry.Policies.Global.Add<AsyncHandlingConvention>();

            // Just forcing it to get spun up.
            registry.AlterSettings<ChannelGraph>(x => {});

            if (FubuTransport.AllQueuesInMemory)
            {


                registry.Services(x =>
                {
                    x.ClearAll<ITransport>();
                    x.AddService<ITransport, InMemoryTransport>();

                    SettingTypes.Each(settingType =>
                    {
                        var settings = InMemoryTransport.ToInMemory(settingType);
                        x.ReplaceService(settingType, new ObjectInstance(settings));
                    });
                });
            }

            if (FubuTransport.AllQueuesInMemory || EnableInMemoryTransport)
            {
                registry.Services(_ => _.AddService<ITransport, InMemoryTransport>());
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
                registry.AlterSettings<T>(x => InMemoryTransport.AllChannelsAreInMemory(typeof(T), x));
            }
        }
    }

    
}