using FubuMVC.Core;
using FubuMVC.Core.StructureMap;

namespace SerenityDemonstrator
{
    public class DemonstratorApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            FubuMode.SetUpForDevelopmentMode();
            return FubuApplication.DefaultPolicies();
        }
    }
}