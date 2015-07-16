using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
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
                ;
        }
    }
    // ENDSAMPLE
}