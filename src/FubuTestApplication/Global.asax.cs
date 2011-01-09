using FubuMVC.Core;
using FubuMVC.StructureMap.Bootstrap;

namespace FubuTestApplication
{
    public class Global : FubuStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            return new FubuTestApplicationRegistry();
        }
    }

    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Actions.IncludeType<ScriptsHandler>();
        }
    }
}