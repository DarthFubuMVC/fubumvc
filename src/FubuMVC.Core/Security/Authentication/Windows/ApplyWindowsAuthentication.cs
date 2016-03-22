namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class ApplyWindowsAuthentication : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Actions.FindWith<WindowsActionSource>();
            registry.Services.IncludeRegistry<WindowsAuthenticationServiceRegistry>();
        }
    }
}