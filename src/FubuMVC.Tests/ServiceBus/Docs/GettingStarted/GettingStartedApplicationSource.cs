using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.StructureMap;
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