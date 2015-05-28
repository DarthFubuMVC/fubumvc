using FubuCore.Descriptions;
using FubuMVC.Core;

namespace FubuMVC.Authentication.Endpoints
{
    [Title("Adds the default endpoints for basic authentication if they do not already exist in the BehaviorGraph")]
    public class FormsAuthenticationRegistry : FubuPackageRegistry
    {
        public FormsAuthenticationRegistry()
        {
            Actions.IncludeType<LoginController>();
            Actions.IncludeType<LogoutController>();
        }
    }
}