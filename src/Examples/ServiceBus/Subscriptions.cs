using System;
using Examples.HelloWorld.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.RavenDb.ServiceBus;

namespace Examples.ServiceBus
{
    public class OtherAppMessage1{}
    public class OtherAppMessage2{}
    public class OtherAppMessage3{}

    // SAMPLE: NodeSettings
    public class NodeSettings
    {
        // This uri points to a different
        // application
        public Uri OtherApp { get; set; }

        // This uri should be the shared
        // channel that all nodes in the
        // application cluster are reading
        public Uri Receiving { get; set; }
    }
    // ENDSAMPLE

    // SAMPLE: configuring-subscriptions
    public class LocalApp : FubuTransportRegistry<NodeSettings>
    {
        public LocalApp()
        {
            // Explicitly set the logical descriptive
            // name of this application. The default is
            // derived from the name of the class
            NodeName = "MyApplication";

            // Incoming messages
            Channel(x => x.Receiving)
                .ReadIncoming();

            // Local subscription to only this node
            SubscribeLocally()
                .ToSource(x => x.OtherApp)
                .ToMessage<OtherAppMessage1>();

            // Global subscription to the all the
            // running nodes in this clustered application
            SubscribeAt(x => x.Receiving)
                .ToSource(x => x.OtherApp)
                .ToMessage<OtherAppMessage2>()
                .ToMessage<OtherAppMessage3>();
        }
    }
    // ENDSAMPLE

    // SAMPLE: SubscriptionStorageOverride
    public class SubscriptionStorageApp : FubuTransportRegistry<AppSettings>
    {
        public SubscriptionStorageApp()
        {
            // Plug in subscription storage backed by RavenDb
            Services
                .ReplaceService<ISubscriptionPersistence, RavenDbSubscriptionPersistence>();
        }
    }
    // ENDSAMPLE
}