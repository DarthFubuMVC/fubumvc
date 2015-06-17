using FubuTransportation.Configuration;

namespace FubuTransportation.Testing.Docs.GettingStarted
{
    // SAMPLE: GettingStartedTransportRegistry
    public class GettingStartedTransportRegistry : FubuTransportRegistry<GettingStartedSettings>
    {
        public GettingStartedTransportRegistry()
        {
            // You have to have at least one transport registered,
            // so we're enabling the in memory transport for this sample;)
            EnableInMemoryTransport();

            Channel(x => x.Uri)
                //Routes messages in the in the getting started namespace to this channel
                .AcceptsMessages(x => typeof(GettingStartedSettings).Namespace.Equals(x.Namespace))
                .ReadIncoming();
        }
    }
    // ENDSAMPLE
}