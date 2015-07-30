using FubuMVC.Core;

namespace FubuMVC.Tests.ServiceBus.Docs.GettingStarted
{
    // SAMPLE: GettingStartedApplicationSource
    public class GettingStartedApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory = null)
        {
            return FubuApplication.For<GettingStartedTransportRegistry>()
                ;
        }
    }
    // ENDSAMPLE
}