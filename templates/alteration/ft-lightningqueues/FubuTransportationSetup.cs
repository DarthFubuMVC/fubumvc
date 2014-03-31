using System;
using FubuTransportation;
using FubuTransportation.Configuration;

namespace %NAMESPACE%
{
    public class BusSettings
    {
        public BusSettings()
        {
            Inbound = new Uri("lq.tcp://localhost:2200/inbound");
            Outbound = new Uri("lq.tcp://localhost:2201/outbound");
        }

        public Uri Inbound { get; set; }
        public Uri Outbound { get; set; }
    }

    public class %TRANSPORT_REGISTRY% : FubuTransportRegistry<BusSettings>
    {
        public %TRANSPORT_REGISTRY%()
        {
            // More options are available for thread or task scheduling
            Channel(x => x.Inbound).ReadIncoming();

            // Static subscriptions
            Channel(x => x.Outbound)
                .AcceptsMessages(x => true);
        }
    }
}