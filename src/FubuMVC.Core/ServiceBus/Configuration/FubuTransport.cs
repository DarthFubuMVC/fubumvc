using System;
using FubuCore;
using FubuMVC.Core.Registration;
using StructureMap;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    /// <summary>
    /// Use to bootstrap a FubuTransportation application that is not co-hosted with a FubuMVC
    /// application
    /// </summary>
    public static class FubuTransport
    {
        public static BehaviorGraph BehaviorGraphFor(Action<FubuRegistry> configuration)
        {
            return BehaviorGraph.BuildFrom(x =>
            {
                x.Features.ServiceBus.Enable(true);
                configuration(x);
            });
        }

        public static FubuApplication DefaultPolicies(IContainer container = null)
        {
            var registry = new FubuRegistry();
            if (container != null) registry.StructureMap(container);

            registry.Features.ServiceBus.Configure(x =>
            {
                x.Enabled = true;
                x.EnableInMemoryTransport = true;
            });

            return FubuApplication.For(registry);
        }

        static FubuTransport()
        {
            Reset();
        }

        public static void Reset()
        {
            UseSynchronousLogging = ApplyMessageHistoryWatching = AllQueuesInMemory = false;
        }

        public static bool UseSynchronousLogging { get; set; }

        public static bool ApplyMessageHistoryWatching { get; set; }

        public static bool AllQueuesInMemory { get; set; }

        public static void SetupForInMemoryTesting()
        {
            UseSynchronousLogging = ApplyMessageHistoryWatching = AllQueuesInMemory = true;
        }

        /// <summary>
        /// Configures FT to use in-memory queues system-wide and apply message 
        /// history watching.  Any properties in Settings classes used with 
        /// ExternalNodes that match properties on <typeparamref name="TSettings"/> 
        /// will be automatically configured to match.
        /// </summary>
        /// <typeparam name="TSettings">The settings type used by the system under test.</typeparam>
        public static void SetupForInMemoryTesting<TSettings>()
        {
            SetupForInMemoryTesting();
            DefaultSettings = typeof(TSettings);
        }

        internal static Type DefaultSettings { get; set; }
        internal static ChannelGraph DefaultChannelGraph { get; set; }

        public static void SetupForTesting()
        {
            FubuMode.SetupForTestingMode();
            UseSynchronousLogging = true;
            ApplyMessageHistoryWatching = true;
        }
    }
}
