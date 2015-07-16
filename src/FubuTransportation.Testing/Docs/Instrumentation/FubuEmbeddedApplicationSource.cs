using FubuMVC.Core;

namespace FubuTransportation.Testing.Docs.Instrumentation
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