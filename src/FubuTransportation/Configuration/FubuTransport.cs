using System;
using FubuMVC.Core;
using FubuCore;
using FubuMVC.Core.Registration.DSL;

namespace FubuTransportation.Configuration
{
    /// <summary>
    /// Use to bootstrap a FubuTransportation application that is not co-hosted with a FubuMVC
    /// application
    /// </summary>
    public static class FubuTransport
    {
        public static IContainerFacilityExpression For<T>() where T : FubuTransportRegistry, new()
        {
            var extension = new T();

            return For(extension);
        }

        public static IContainerFacilityExpression For(FubuTransportRegistry extension)
        {
            var registry = new FubuRegistry();
            extension.As<IFubuRegistryExtension>().Configure(registry);
            return FubuApplication.For(registry);
        }

        public static IContainerFacilityExpression For(Action<FubuTransportRegistry> configuration)
        {
            var extension = FubuTransportRegistry.For(configuration);
            return For(extension);
 
        }

        public static IContainerFacilityExpression DefaultPolicies()
        {
            return For(x => {
                x.EnableInMemoryTransport();
            });
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
