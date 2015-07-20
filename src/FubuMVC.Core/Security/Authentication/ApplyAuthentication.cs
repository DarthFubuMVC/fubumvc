namespace FubuMVC.Core.Security.Authentication
{
    public class ApplyAuthentication : IFubuRegistryExtension
    {
        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Services<AuthenticationServiceRegistry>();

            registry.Policies.Global.Add(new ApplyAuthenticationPolicy());
            registry.Policies.Global.Add<RegisterAuthenticationStrategies>();
            registry.Policies.Global.Add<ApplyPassThroughAuthenticationPolicy>();
        }
    }
}