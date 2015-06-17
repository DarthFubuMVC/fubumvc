using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuTransportation.Sagas;

namespace FubuTransportation
{
    [ApplicationLevel]
    public class TransportSettings
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
            Disabled = false;
        }

        public bool Disabled { get; set; }
        public bool EnableInMemoryTransport { get; set; }
        public bool DebugEnabled { get; set; }
        public double DelayMessagePolling { get; set; }
        public double ListenerCleanupPolling { get; set; }
        public double SubscriptionRefreshPolling { get; set; }
    }
}