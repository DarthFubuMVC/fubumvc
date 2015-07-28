using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Katana;

namespace FubuMVC.Tests.ServiceBus.Docs.GettingStarted
{
    // SAMPLE: GettingStartedTransportRegistry
    public class GettingStartedTransportRegistry : FubuTransportRegistry<GettingStartedSettings>
    {
        public GettingStartedTransportRegistry()
        {
            // You have to have at least one transport registered,
            // so we're enabling the in memory transport for this sample;)
            ServiceBus.EnableInMemoryTransport();

            Channel(x => x.Uri)
                //Routes messages in the in the getting started namespace to this channel
                .AcceptsMessages(x => typeof(GettingStartedSettings).Namespace.Equals(x.Namespace))
                .ReadIncoming();

            AlterSettings<KatanaSettings>(x =>
            {
                x.Port = 5500;
                x.AutoHostingEnabled = true;
            });
        }
    }
    // ENDSAMPLE
}