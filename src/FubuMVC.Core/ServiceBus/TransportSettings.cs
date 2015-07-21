using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Sagas;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration;
using FubuMVC.Core.ServiceBus.Web;

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

            registry.Import<BuiltInPollingJobRegistry>();


            registry.Policies.Global.Add<ApplyScheduledJobRouting>();
            registry.Services<ScheduledJobServicesRegistry>();
            registry.Services<MonitoringServiceRegistry>();
            registry.Policies.Global.Add<RegisterScheduledJobs>();
            registry.Policies.ChainSource<ImportHandlers>();
            registry.Services<FubuTransportServiceRegistry>();
            registry.Services<PollingServicesRegistry>();
            registry.Policies.Global.Add<RegisterPollingJobs>();
            registry.Policies.Global.Add<StatefulSagaConvention>();
            registry.Policies.Global.Add<AsyncHandlingConvention>();

            if (FubuTransport.AllQueuesInMemory)
            {
                registry.Policies.Global.Add<AllQueuesInMemoryPolicy>();
            }

            registry.Policies.Global.Add<InMemoryQueueRegistration>();

            registry.Policies.Global.Add<ReorderBehaviorsPolicy>(x =>
            {
                x.ThisNodeMustBeBefore<StatefulSagaNode>();
                x.ThisNodeMustBeAfter<HandlerCall>();
            });
        }
    }
}