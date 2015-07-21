using System;
using FubuCore;
using StructureMap;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    /// <summary>
    /// Use to bootstrap a FubuTransportation application that is not co-hosted with a FubuMVC
    /// application
    /// </summary>
    public static class FubuTransport
    {
        public static FubuApplication For<T>(Action<T> customize = null, IContainer container = null) where T : FubuTransportRegistry, new()
        {
            var extension = new T();
            if (customize != null)
            {
                customize(extension);
            }

            return For(extension, container);
        }

        public static FubuApplication For<T>(IContainer container) where T : FubuTransportRegistry, new()
        {
            var registry = new FubuRegistry();
            registry.StructureMap(container);
            var extension = new T();
            extension.As<IFubuRegistryExtension>().Configure(registry);
            return FubuApplication.For(registry);
        }

        public static FubuApplication For(FubuTransportRegistry extension, IContainer container = null)
        {
            var registry = new FubuRegistry();
            if (container != null)
            {
                registry.StructureMap(container);
            }

            extension.As<IFubuRegistryExtension>().Configure(registry);
            return FubuApplication.For(registry);
        }

        public static FubuApplication For(Action<FubuTransportRegistry> configuration, IContainer container = null)
        {
            var extension = FubuTransportRegistry.For(configuration);
            return For(extension, container);
 
        }

        public static FubuApplication DefaultPolicies(IContainer container = null)
        {
            return For(x => {
                x.EnableInMemoryTransport();
            }, container);
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
