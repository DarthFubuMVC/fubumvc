namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Add<MenuItemAttributeConfigurator>();
            registry.Policies.Add<CompileNavigationStep>();

            registry.Services<NavigationServiceRegistry>();
        }
    }
}