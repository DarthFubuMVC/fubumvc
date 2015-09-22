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

        public static bool AllQueuesInMemory { get; set; }

        public static void SetupForInMemoryTesting()
        {
            AllQueuesInMemory = true;
        }


        internal static Type DefaultSettings { get; set; }
        internal static ChannelGraph DefaultChannelGraph { get; set; }

    }
}
