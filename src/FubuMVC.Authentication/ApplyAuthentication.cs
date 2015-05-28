using FubuMVC.Core;

namespace FubuMVC.Authentication
{
    public class ApplyAuthentication : IFubuRegistryExtension
    {
        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Policies.Global.Add<RegisterAuthenticationStrategies>();

            registry.Services<AuthenticationServiceRegistry>();
            registry.Policies.Global.Add(new ApplyAuthenticationPolicy());

            registry.Policies.Global.Add<ApplyPassThroughAuthenticationPolicy>();
        }
    }
}