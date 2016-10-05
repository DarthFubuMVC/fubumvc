using System;
using Examples.HelloWorld.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;

namespace Examples.ServiceBus
{
    // SAMPLE: LqApp
    public class LqAppSettings
    {
        public Uri Incoming { get; set; }
            = new Uri("lq.tcp://localhost:2200/incoming");

        public Uri Control { get; set; }
        = new Uri("lq.tcp://localhost:2201/control");

        public Uri Other { get; set; }
            = new Uri("lq.tcp://localhost:2202/messages");
    }

    public class LqApp : FubuTransportRegistry<LqAppSettings>
    {
        public LqApp()
        {
            // You need at least one incoming channel if you
            // are going to use LightningQueues as a transport
            Channel(x => x.Incoming).ReadIncoming();

            // Designate a control channel and
            // mark it as non-persistent
            Channel(x => x.Control)
                .UseAsControlChannel()
                .DeliveryFastWithoutGuarantee();


            Channel(x => x.Other)
                .AcceptsMessage<PingMessage>();
        }
    }
    // ENDSAMPLE
}