using FubuMVC.Core;
using FubuMVC.Tests.ServiceBus.Docs.GettingStarted;

namespace FubuMVC.Tests.ServiceBus.Docs.Instrumentation
{
    // SAMPLE: FubuEmbeddedApplicationSourceSample
    public class FubuEmbeddedApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication.For<GettingStartedTransportRegistry>();
        }
    }

    // ENDSAMPLE
}