using FubuMVC.Core;
using FubuMVC.Core.StructureMap;

namespace SerenityDemonstrator
{
    public class DemonstratorApplication : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            FubuMode.SetUpForDevelopmentMode();
            return FubuApplication.DefaultPolicies();
        }
    }
}