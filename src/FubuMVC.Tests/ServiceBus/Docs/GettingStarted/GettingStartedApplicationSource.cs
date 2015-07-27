using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Tests.ServiceBus.Docs.GettingStarted
{
    // SAMPLE: GettingStartedApplicationSource
    public class GettingStartedApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuTransport.For<GettingStartedTransportRegistry>()
                ;
        }
    }
    // ENDSAMPLE
}