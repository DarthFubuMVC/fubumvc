using FubuMVC.Core;
using StructureMap;

namespace DiagnosticsHarness
{
    public class HarnessApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            var container = new Container(x => x.ForSingletonOf<INumberCache>().Use<NumberCache>());

            return FubuApplication.For<HarnessRegistry>(_ => _.StructureMap(container));
        }
    }
}