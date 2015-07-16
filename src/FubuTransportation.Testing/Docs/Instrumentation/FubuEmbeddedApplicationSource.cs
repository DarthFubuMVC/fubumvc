using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using StructureMap;

namespace FubuTransportation.Testing.Docs.Instrumentation
{
    // SAMPLE: FubuEmbeddedApplicationSourceSample
    public class FubuEmbeddedApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<FubuEmbeddedRegistry>()
                .StructureMap(new Container());
        }
    }
    // ENDSAMPLE
}