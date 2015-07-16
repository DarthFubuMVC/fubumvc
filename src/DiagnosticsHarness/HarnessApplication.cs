using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using StructureMap;

namespace DiagnosticsHarness
{
    public class HarnessApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            var container = new Container(x => { x.ForSingletonOf<INumberCache>().Use<NumberCache>(); });

            return FubuApplication.For<FubuHarnessRegistry>().StructureMap(container);
            //return FubuTransport.For<HarnessRegistry>().StructureMap(container);
        }
    }
}