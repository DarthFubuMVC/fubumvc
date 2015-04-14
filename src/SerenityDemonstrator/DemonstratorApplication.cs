using FubuMVC.Core;
using FubuMVC.StructureMap;

namespace SerenityDemonstrator
{
    public class DemonstratorApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.DefaultPolicies().StructureMap();
        }
    }
}