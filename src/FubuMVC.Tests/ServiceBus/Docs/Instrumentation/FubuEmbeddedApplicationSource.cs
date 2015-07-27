using FubuMVC.Core;

namespace FubuMVC.Tests.ServiceBus.Docs.Instrumentation
{
    // SAMPLE: FubuEmbeddedApplicationSourceSample
    public class FubuEmbeddedApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<FubuEmbeddedRegistry>();
        }
    }

    // ENDSAMPLE
}