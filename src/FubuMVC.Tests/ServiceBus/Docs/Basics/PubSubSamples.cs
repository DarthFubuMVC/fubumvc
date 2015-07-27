using System;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Tests.ServiceBus.Docs.Basics
{
    // SAMPLE: PubSubRoutingSample
    public class AllEndpointsSettings
    {
        public Uri RemoteEndpoint { get; set; }
        public Uri AnotherRemoteEndpoint { get; set; }
        public Uri ThisEndpoint { get; set; }
    }

    public class RoutingSampleTransportRegistry : FubuTransportRegistry<AllEndpointsSettings>
    {
        public RoutingSampleTransportRegistry()
        {
            //route to remote endpoint all messages in this assembly
            Channel(x => x.RemoteEndpoint)
                .AcceptsMessagesInAssemblyContainingType<SampleMessage>();

            //route all messages suffixed with Event to AnotherRemoteEndpoint
            Channel(x => x.AnotherRemoteEndpoint)
                .AcceptsMessages(x => x.Name.EndsWith("Event"));

            //Route messages suffixed with Command to ThisEndpoint
            Channel(x => x.ThisEndpoint)
                .AcceptsMessages(x => x.Name.EndsWith("Command"))
                //read messages from this 'Incoming' channel
                .ReadIncoming();
        }
    }
    // ENDSAMPLE

    public class SampleMessage
    {
    }
}