using System;
using System.Data;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;


namespace DiagnosticsHarness
{
    public class HarnessRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public HarnessRegistry()
        {
            //Comment out in order to test with LightningQueues
            //EnableInMemoryTransport();

            Features.Diagnostics.Enable(TraceLevel.Verbose);

            Services.ForSingletonOf<INumberCache>().Use<NumberCache>();

            // TODO -- publish everything option in the FI?
            Channel(x => x.Channel).ReadIncoming().AcceptsMessages(x => true).DefaultSerializer<XmlMessageSerializer>();

            Policies.Global.Add<ErrorHandlingPolicy>();

            SubscribeLocally().ToSource(x => x.Publisher)
                .ToMessage<NumberMessage>();
        }
    }

    public class ErrorHandlingPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain chain)
        {
            chain.MaximumAttempts = 5;
            chain.OnException<TimeoutException>().Retry();
            chain.OnException<DBConcurrencyException>().Retry();
        }
    }
}