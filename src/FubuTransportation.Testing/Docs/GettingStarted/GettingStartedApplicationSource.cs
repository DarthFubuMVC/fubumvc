using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTransportation.Configuration;
using StructureMap;

namespace FubuTransportation.Testing.Docs.GettingStarted
{
    // SAMPLE: GettingStartedApplicationSource
    public class GettingStartedApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuTransport.For<GettingStartedTransportRegistry>()
                .StructureMap(new Container());
        }
    }
    // ENDSAMPLE
}